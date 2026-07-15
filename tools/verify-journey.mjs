// End-to-end journey through the HomeBuyerHelper web app.
// Run:  node tools/verify-journey.mjs [baseUrl]
// Expects the app running (dotnet run --project src/HomeBuyerHelper.Web)
// and Playwright available (globally installed, or PLAYWRIGHT_MODULE set
// to the playwright package entry point).

const PLAYWRIGHT = process.env.PLAYWRIGHT_MODULE
    ?? '/opt/node22/lib/node_modules/playwright/index.mjs';
const { chromium } = await import(PLAYWRIGHT);

const BASE = process.argv[2] ?? 'http://localhost:5191';
const SHOTS = process.env.SHOTS_DIR ?? 'verify-shots';
import { mkdirSync } from 'node:fs';
mkdirSync(SHOTS, { recursive: true });

let failures = 0;
const log = (...a) => console.log('[journey]', ...a);
const fail = (...a) => { failures++; console.error('[FAIL]', ...a); };
const expect = (cond, label) => cond ? log('ok:', label) : fail(label);

const browser = await chromium.launch();
const page = await browser.newPage({ viewport: { width: 1440, height: 1000 } });
page.on('pageerror', e => fail('pageerror:', e.message));
page.on('console', m => { if (m.type() === 'error') fail('console.error:', m.text()); });

const setField = async (label, value, scope = page) => {
    const el = scope.locator(
        `span:has(> label:text-is("${label}")) input, ` +
        `span:has(> label:text-is("${label}")) select, ` +
        `span:has(> label:text-is("${label}")) textarea`).first();
    const tag = await el.evaluate(n => n.tagName);
    const type = tag === 'INPUT' ? await el.getAttribute('type') : null;
    if (type === 'checkbox') { if (value) await el.check(); else await el.uncheck(); }
    else if (tag === 'SELECT') await el.selectOption(String(value));
    else await el.fill(String(value));
};

// --- 1. Fresh visit redirects to onboarding; skipping lands on dashboard
await page.goto(BASE, { waitUntil: 'networkidle' });
await page.waitForURL('**/welcome');
expect(page.url().includes('welcome'), 'first run redirects to onboarding');
await page.screenshot({ path: `${SHOTS}/welcome.png` });
await page.click('button:text-is("Skip for now")');
await page.waitForSelector('h1:has-text("Dashboard")');
expect((await page.textContent('.app-main')).includes('three steps'), 'skipped onboarding shows getting-started steps');

// --- 2. Criteria via template
await page.goto(BASE + '/criteria', { waitUntil: 'networkidle' });
await page.click('text=Apply Template');
// Confirm step may appear when criteria already exist; first run has none.
await page.waitForSelector('.criteria-item');
expect((await page.textContent('h2 .badge')).includes('100%'), 'template weights total 100%');

// --- 3. Two properties, scored
await page.goto(BASE + '/properties', { waitUntil: 'networkidle' });
for (const [nick, price, commute] of [['Craftsman on 5th', '525000', '50'], ['Condo Downtown', '410000', '15']]) {
    await page.click('text=Add Property');
    await setField('Nickname *', nick);
    await setField('Asking Price', price);
    await setField('Commute RT min', commute);
    await page.click('text=Save Property');
    await page.waitForSelector(`.property-card:has-text("${nick}")`);
}
for (const nick of ['Craftsman on 5th', 'Condo Downtown']) {
    await page.click(`.property-card:has-text("${nick}") >> button:text-is("Score")`);
    await page.waitForSelector('h2:has-text("Score:")');
    const items = page.locator('.card:has(h2:has-text("Score:")) .score-item');
    const n = await items.count();
    for (let i = 0; i < n; i++) {
        const score = nick.startsWith('Craftsman') ? 5 + (i % 5) : 4 + (i % 6);
        await items.nth(i).locator(`.score-picker button:text-is("${score}")`).click();
    }
    await page.click('text=Save Scores');
    await page.waitForSelector('h2:has-text("Score:")', { state: 'detached' });
}
log('two properties added and scored');

// --- 4. Rankings on dashboard
await page.goto(BASE, { waitUntil: 'networkidle' });
await page.waitForSelector('table');
expect((await page.textContent('table')).includes('Craftsman'), 'dashboard ranks properties');
await page.screenshot({ path: `${SHOTS}/dashboard.png` });

// --- 5. Compare shows scores + true total cost, no native-app copy
await page.goto(BASE + '/compare', { waitUntil: 'networkidle' });
await page.waitForSelector('h2:has-text("True Total Cost")');
expect(!(await page.textContent('.app-main')).includes('native app'), 'no native-app dead-end copy');
await page.screenshot({ path: `${SHOTS}/compare.png`, fullPage: true });

// --- 6. Budget: income + expense → projection
await page.goto(BASE + '/budget', { waitUntil: 'networkidle' });
const incomeCard = page.locator('.card:has(h2:text-is("Income Sources"))');
await incomeCard.locator('.preset-row button:text-is("Salary")').click();
await incomeCard.locator('span:has(> label:text-is("Amount")) input').fill('12500');
await incomeCard.locator('button:text-is("Add")').click();
await page.waitForSelector('.card:has(h2:text-is("Income Sources")) .score-item:has-text("Salary")');
const expenseCard = page.locator('.card:has(h2:text-is("Expenses"))');
await expenseCard.locator('.preset-row button:text-is("Rent")').click();
await expenseCard.locator('span:has(> label:text-is("Amount/mo")) input').fill('2800');
await expenseCard.locator('button:text-is("Add")').click();
await page.waitForSelector('.card:has(h2:text-is("Expenses")) .score-item:has-text("Rent")');
expect(await page.locator('h2:has-text("24-Month Cash Flow")').count() === 1, 'cash flow projection renders');
expect(await page.locator('.stat-strip .stat').count() === 4, 'projection insight strip renders');
await page.click('button:has-text("Show all")');
await page.waitForFunction(() => document.querySelectorAll('table tbody tr').length > 6);
log('projection expands to all months');

// --- 7. Funding
await page.goto(BASE + '/funding', { waitUntil: 'networkidle' });
const addCard = page.locator('.card:has(h2:text-is("Add a Source"))');
await addCard.locator('span:has(> label:text-is("Nickname")) input').fill('Chase Savings');
await addCard.locator('span:has(> label:text-is("Amount")) input').fill('60000');
await addCard.locator('button:text-is("Add Source")').click();
await page.waitForSelector('h2:has-text("Your Funding Plan")');
log('funding plan renders');

// --- 8. Offers
await page.goto(BASE + '/offers', { waitUntil: 'networkidle' });
await page.click('text=Start from asking price');
await page.waitForSelector('table');
expect((await page.textContent('table')).includes('Cash to close'), 'offer evaluation renders');
await page.screenshot({ path: `${SHOTS}/offers.png`, fullPage: true });

// --- 8b. My Plan: set on Offers, verify Budget/Funding/Dashboard react
await page.click('button:text-is("Set as plan")');
await page.waitForSelector('.badge.ok:has-text("Plan")');
log('plan set on offer');

await page.goto(BASE + '/budget', { waitUntil: 'networkidle' });
expect((await page.textContent('.app-main')).includes('Projection includes'),
    'budget projection announces the plan');

await page.goto(BASE + '/funding', { waitUntil: 'networkidle' });
const fundingBody = await page.textContent('.app-main');
expect(/cash to close|covers|shortfall|surplus/i.test(fundingBody),
    'funding shows plan coverage');

await page.goto(BASE, { waitUntil: 'networkidle' });
expect((await page.textContent('.app-main')).includes('Your Plan'),
    'dashboard shows Your Plan card');
await page.screenshot({ path: `${SHOTS}/plan-dashboard.png`, fullPage: true });

// Probe: deleting the planned property must self-heal, not crash
// (covered by unit tests; here we just confirm clear works from the UI)
await page.goto(BASE + '/offers', { waitUntil: 'networkidle' });
await page.click('button:text-is("Clear plan")');
await page.waitForSelector('button:text-is("Set as plan")');
await page.goto(BASE, { waitUntil: 'networkidle' });
expect(!(await page.textContent('.app-main')).includes('Your Plan'),
    'clearing the plan removes the dashboard card');
await page.goto(BASE + '/offers', { waitUntil: 'networkidle' });
await page.click('button:text-is("Set as plan")');
await page.waitForSelector('.badge.ok:has-text("Plan")');

// --- 9. Rent vs Buy
await page.goto(BASE + '/rent-vs-buy', { waitUntil: 'networkidle' });
await setField('Purchase Price', '525000');
await setField('Current Rent/mo', '2800');
await page.click('button:has-text("Compare")');
await page.waitForSelector('.verdict-card');
log('rent-vs-buy verdict renders');

// --- 9a. Scenarios: buy-now vs wait comparison (prefilled from the plan)
await page.goto(BASE + '/scenarios', { waitUntil: 'networkidle' });
await page.click('button:has-text("Compare")');
await page.waitForSelector('.verdict-card');
expect((await page.textContent('table')).toLowerCase().includes('cash to close'),
    'scenarios table renders buy-now vs wait');
await page.screenshot({ path: `${SHOTS}/scenarios.png`, fullPage: true });

// --- 9a2. Report: sections render, CSVs download
await page.goto(BASE + '/report', { waitUntil: 'networkidle' });
const reportText = await page.textContent('.app-main');
expect(reportText.includes('Your Plan'), 'report shows plan section');
expect(reportText.includes('Property Comparison'), 'report shows comparison');
expect(/cash flow/i.test(reportText), 'report shows cash flow');
const csvDownload = page.waitForEvent('download');
await page.click('button:has-text("Comparison CSV")');
const csv = await csvDownload;
expect((await csv.suggestedFilename()).endsWith('.csv'), 'comparison CSV downloads');
await page.screenshot({ path: `${SHOTS}/report.png`, fullPage: true });

// --- 9b. Settings: change a default and see it flow into calculations
await page.goto(BASE + '/settings', { waitUntil: 'networkidle' });
await setField('Interest Rate %', '6.0');
await page.click('button:has-text("Save Settings")');
await page.waitForSelector('.badge.ok:has-text("Saved")');
await page.goto(BASE + '/offers', { waitUntil: 'networkidle' });
expect((await page.textContent('.card')).length > 0, 'offers loads after settings change');
const rateField = page.locator('span:has(> label:text-is("Rate %")) input').first();
expect(await rateField.inputValue() === '6.0', 'offer form defaults to saved 6.0% rate');

// --- 9c. Criteria template confirm guard (criteria already exist now)
await page.goto(BASE + '/criteria', { waitUntil: 'networkidle' });
await page.click('text=Apply Template');
expect(await page.locator('text=/replaces your \\d+ current criteria/i').count() > 0,
    'template apply asks for confirmation when criteria exist');
await page.click('button:text-is("Cancel")');

// --- 9d. Backup round-trip: export → delete all → import → data restored
await page.goto(BASE + '/settings/data', { waitUntil: 'networkidle' });
const downloadPromise = page.waitForEvent('download');
await page.click('text=Download backup');
const download = await downloadPromise;
const backupPath = `${SHOTS}/backup.json`;
await download.saveAs(backupPath);
expect((await page.textContent('.badge.brand')).includes('Last backup:'), 'last-backup stamp updates');

await page.fill('input[placeholder="DELETE"]', 'DELETE');
await page.click('button:text-is("Delete all data")');
await page.waitForLoadState('networkidle');
await page.goto(BASE, { waitUntil: 'networkidle' });
await page.waitForURL('**/welcome');
expect(page.url().includes('welcome'), 'delete-all resets to first-run onboarding');

await page.goto(BASE + '/settings/data', { waitUntil: 'networkidle' });
await page.setInputFiles('input[type="file"]', backupPath);
await page.waitForSelector('input[placeholder="REPLACE"]');
await page.fill('input[placeholder="REPLACE"]', 'REPLACE');
await page.click('button:text-is("Replace all data")');
await page.waitForLoadState('networkidle');
await page.goto(BASE, { waitUntil: 'networkidle' });
await page.waitForSelector('table');
expect((await page.textContent('table')).includes('Craftsman'), 'import restores properties');

// --- 10. Mobile reachability: everything via tab bar or dashboard cards
const mobile = await browser.newPage({ viewport: { width: 390, height: 844 } });
await mobile.goto(BASE, { waitUntil: 'networkidle' });
// Fresh context lands on onboarding (client-side redirect arrives after
// goto resolves — wait for the page, not the URL); skip to reach the grid.
await mobile.waitForSelector('button:text-is("Skip for now")');
await mobile.click('button:text-is("Skip for now")');
await mobile.waitForSelector('h1:has-text("Dashboard")');
await mobile.waitForSelector('.module-card'); // async init done: module grid rendered
const reachable = await mobile.$$eval('.tab-bar a, .module-card', els => els.map(e => e.getAttribute('href')));
log('mobile-reachable routes:', reachable.join(', '));
for (const route of ['offers', 'settings', 'funding', 'rent-vs-buy', 'scenarios']) {
    expect(reachable.includes(route), `mobile can reach /${route}`);
}
await mobile.close();

await browser.close();
if (failures > 0) {
    console.error(`JOURNEY FAILED — ${failures} failure(s)`);
    process.exit(1);
}
console.log('JOURNEY PASSED');
