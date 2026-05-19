const maps = {};

function loadCss(href) {
    return new Promise((resolve, reject) => {
        if ([...document.styleSheets].some(x => x.href && x.href.includes(href))) {
            resolve();
            return;
        }

        const link = document.createElement("link");
        link.rel = "stylesheet";
        link.href = href;
        link.onload = resolve;
        link.onerror = reject;
        document.head.appendChild(link);
    });
}

function loadScript(src) {
    return new Promise((resolve, reject) => {
        if ([...document.scripts].some(x => x.src && x.src.includes(src))) {
            resolve();
            return;
        }

        const script = document.createElement("script");
        script.src = src;
        script.onload = resolve;
        script.onerror = reject;
        document.body.appendChild(script);
    });
}

async function ensureLeaflet() {
    if (window.L)
        return;

    await loadCss("https://unpkg.com/leaflet@1.9.4/dist/leaflet.css");
    await loadScript("https://unpkg.com/leaflet@1.9.4/dist/leaflet.js");
}

export async function renderLocationMap(mapId, latitude, longitude, label) {
    await ensureLeaflet();

    let entry = maps[mapId];

    if (!entry) {
        const map = L.map(mapId, { zoomControl: true });
        L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
            maxZoom: 19,
            attribution: "&copy; OpenStreetMap"
        }).addTo(map);

        entry = { map, marker: null };
        maps[mapId] = entry;
    }

    const { map } = entry;

    if (entry.marker) {
        map.removeLayer(entry.marker);
    }

    entry.marker = L.marker([latitude, longitude]).addTo(map).bindPopup(label);
    map.setView([latitude, longitude], 15);

    setTimeout(() => map.invalidateSize(), 50);
}

export function disposeLocationMap(mapId) {
    const entry = maps[mapId];
    if (!entry)
        return;

    entry.map.remove();
    delete maps[mapId];
}