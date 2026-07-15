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

// --- 1. Fresh dashboard shows getting-started steps
await page.goto(BASE, { waitUntil: 'networkidle' });
await page.waitForSelector('h1');
expect((await page.textContent('.app-main')).includes('three steps'), 'fresh dashboard shows steps');

// --- 2. Criteria via template
await page.goto(BASE + '/criteria', { waitUntil: 'networkidle' });
await page.click('text=Apply Template');
// Confirm step may appear when criteria already exist; first run has none.
await page.waitForSelector('table');
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
    const inputs = page.locator('.card:has(h2:has-text("Score:")) tbody input');
    const n = await inputs.count();
    for (let i = 0; i < n; i++) {
        await inputs.nth(i).fill(String(nick.startsWith('Craftsman') ? 5 + (i % 5) : 4 + (i % 6)));
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
await incomeCard.locator('span:has(> label:text-is("Name")) input').fill('Salary');
await incomeCard.locator('span:has(> label:text-is("Amount")) input').fill('12500');
await incomeCard.locator('button:text-is("Add")').click();
await page.waitForSelector('.card:has(h2:text-is("Income Sources")) table');
const expenseCard = page.locator('.card:has(h2:text-is("Expenses"))');
await expenseCard.locator('span:has(> label:text-is("Name")) input').fill('Rent');
await expenseCard.locator('span:has(> label:text-is("Amount/mo")) input').fill('2800');
await expenseCard.locator('button:text-is("Add")').click();
await page.waitForSelector('.card:has(h2:text-is("Expenses")) table');
expect(await page.locator('h2:has-text("24-Month Cash Flow")').count() === 1, 'cash flow projection renders');

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

// --- 9. Rent vs Buy
await page.goto(BASE + '/rent-vs-buy', { waitUntil: 'networkidle' });
await setField('Purchase Price', '525000');
await setField('Current Rent/mo', '2800');
await page.click('button:has-text("Compare")');
await page.waitForSelector('.verdict-card');
log('rent-vs-buy verdict renders');

// --- 10. Mobile reachability: everything via tab bar or dashboard cards
const mobile = await browser.newPage({ viewport: { width: 390, height: 844 } });
await mobile.goto(BASE, { waitUntil: 'networkidle' });
await mobile.waitForSelector('.tab-bar');
const reachable = await mobile.$$eval('.tab-bar a, .module-card', els => els.map(e => e.getAttribute('href')));
log('mobile-reachable routes:', reachable.join(', '));
await mobile.close();

await browser.close();
if (failures > 0) {
    console.error(`JOURNEY FAILED — ${failures} failure(s)`);
    process.exit(1);
}
console.log('JOURNEY PASSED');
