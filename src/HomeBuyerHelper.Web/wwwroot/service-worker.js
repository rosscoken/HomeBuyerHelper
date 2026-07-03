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
const IS_DEV = ['localhost', '127.0.0.1'].includes(self.location.hostname);

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
            ? handleNavigation(event.request)
            : staleWhileRevalidate(event.request)
    );
});

// The server answers every route with the SPA shell (index.html / 404.html),
// so one cached copy under a fixed key serves any offline navigation.
async function handleNavigation(request) {
    const cache = await caches.open(CACHE);
    try {
        const fresh = await fetch(request);
        if (fresh.ok) {
            await cache.put(SHELL_KEY, fresh.clone());
        }
        return fresh;
    } catch {
        const shell = await cache.match(SHELL_KEY);
        return shell ?? Response.error();
    }
}

async function staleWhileRevalidate(request) {
    const cache = await caches.open(CACHE);
    const cached = await cache.match(request);
    const refresh = fetch(request)
        .then(response => {
            if (response.ok) {
                cache.put(request, response.clone());
            }
            return response;
        })
        .catch(() => undefined);
    if (cached) {
        return cached;
    }
    return (await refresh) ?? Response.error();
}
