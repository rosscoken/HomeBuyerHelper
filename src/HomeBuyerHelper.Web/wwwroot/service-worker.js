// Offline support: cache-as-you-go, no build-time asset manifest.
//
// Blazor's stock PWA service worker relies on an integrity-checked assets
// manifest generated at publish time — which the Pages deploy would break,
// because it rewrites <base href> in index.html *after* publish. This worker
// avoids that entirely:
//
//  - Navigations are network-first and always cache the latest app shell,
//    so updates land whenever the user is online, and any route still loads
//    the shell when offline.
//  - Everything else same-origin is stale-while-revalidate. Blazor's
//    _framework files are content-hashed (immutable), so serving from cache
//    is always correct; unhashed files refresh in the background and apply
//    on the next launch.
//
// Net effect: the app works fully offline after its first online visit.

const CACHE = 'homebuyerhelper-v1';
const SHELL_KEY = 'app-shell';
const IS_DEV = ['localhost', '127.0.0.1', '[::1]', '::1'].includes(self.location.hostname);

self.addEventListener('install', () => {
    self.skipWaiting();
});

self.addEventListener('activate', event => {
    event.waitUntil((async () => {
        for (const key of await caches.keys()) {
            if (key !== CACHE) {
                await caches.delete(key);
            }
        }
        await self.clients.claim();
    })());
});

self.addEventListener('fetch', event => {
    if (IS_DEV || event.request.method !== 'GET') {
        return;
    }
    const url = new URL(event.request.url);
    if (url.origin !== self.location.origin) {
        return;
    }
    event.respondWith(
        event.request.mode === 'navigate'
            ? handleNavigation(event)
            : staleWhileRevalidate(event)
    );
});

// The server answers every route with the SPA shell — index.html with a 200,
// or its 404.html copy (deep links on GitHub Pages) with a 404 status — so
// one cached copy under a fixed key serves any offline navigation.
async function handleNavigation(event) {
    const cache = await caches.open(CACHE);
    try {
        const fresh = await fetch(event.request);
        if (fresh.ok || fresh.status === 404) {
            await cache.put(SHELL_KEY, fresh.clone());
        }
        return fresh;
    } catch {
        const shell = await cache.match(SHELL_KEY);
        return shell ?? Response.error();
    }
}

async function staleWhileRevalidate(event) {
    const cache = await caches.open(CACHE);
    const cached = await cache.match(event.request);
    const refresh = fetch(event.request)
        .then(async response => {
            if (response.ok) {
                await cache.put(event.request, response.clone());
            }
            return response;
        })
        .catch(() => undefined);
    if (cached) {
        // Keep the worker alive until the background refresh has been cached.
        event.waitUntil(refresh);
        return cached;
    }
    return (await refresh) ?? Response.error();
}
