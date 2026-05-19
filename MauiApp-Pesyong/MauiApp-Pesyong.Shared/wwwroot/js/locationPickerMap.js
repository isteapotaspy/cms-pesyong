const pickerMaps = {};

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

async function reverseGeocode(lat, lng) {
    try {
        const url =
            `https://nominatim.openstreetmap.org/reverse?format=jsonv2&lat=${lat}&lon=${lng}&addressdetails=1`;

        const response = await fetch(url, {
            headers: {
                "Accept": "application/json"
            }
        });

        if (!response.ok) {
            return {
                streetAddress: "",
                barangay: "",
                city: "",
                displayName: ""
            };
        }

        const data = await response.json();
        const address = data.address || {};

        const houseNumber = address.house_number || "";
        const road = address.road || address.pedestrian || address.footway || "";
        const suburb = address.suburb || address.neighbourhood || address.quarter || address.village || "";
        const city =
            address.city ||
            address.town ||
            address.municipality ||
            address.county ||
            "";

        const streetAddress = [houseNumber, road].filter(Boolean).join(" ").trim();

        return {
            streetAddress,
            barangay: suburb,
            city,
            displayName: data.display_name || ""
        };
    } catch {
        return {
            streetAddress: "",
            barangay: "",
            city: "",
            displayName: ""
        };
    }
}

export async function renderLocationPickerMap(mapId, latitude, longitude, dotNetRef) {
    await ensureLeaflet();

    // Default to Davao City instead of Quiapo / Manila
    const defaultLat = 7.0707;
    const defaultLng = 125.6087;

    const lat = latitude ?? defaultLat;
    const lng = longitude ?? defaultLng;

    let entry = pickerMaps[mapId];

    if (!entry) {
        const map = L.map(mapId, { zoomControl: true });

        L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
            maxZoom: 19,
            attribution: "&copy; OpenStreetMap"
        }).addTo(map);

        const marker = L.marker([lat, lng], { draggable: true }).addTo(map);

        marker.on("dragend", async function () {
            const pos = marker.getLatLng();
            const geo = await reverseGeocode(pos.lat, pos.lng);

            await dotNetRef.invokeMethodAsync(
                "SetPickedLocation",
                pos.lat,
                pos.lng,
                geo.streetAddress,
                geo.barangay,
                geo.city,
                geo.displayName
            );
        });

        map.on("click", async function (e) {
            marker.setLatLng(e.latlng);

            const geo = await reverseGeocode(e.latlng.lat, e.latlng.lng);

            await dotNetRef.invokeMethodAsync(
                "SetPickedLocation",
                e.latlng.lat,
                e.latlng.lng,
                geo.streetAddress,
                geo.barangay,
                geo.city,
                geo.displayName
            );
        });

        map.setView([lat, lng], 14);

        entry = { map, marker };
        pickerMaps[mapId] = entry;
    } else {
        entry.marker.setLatLng([lat, lng]);
        entry.map.setView([lat, lng], 14);
    }

    setTimeout(() => entry.map.invalidateSize(), 50);
}

export function disposeLocationPickerMap(mapId) {
    const entry = pickerMaps[mapId];
    if (!entry)
        return;

    entry.map.remove();
    delete pickerMaps[mapId];
}