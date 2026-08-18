document.addEventListener("htmx:confirm", async function(e) {
  
    const form = e.target.closest('form') || e.target;
    const input = form.querySelector('[name="2be520d4-1fa1-4118-a5a5-627e0576a1c4"]');
    if (!input) return;                 // no special handling
    if (input.value) return;            // already populated

    e.preventDefault();                 // stop default confirm + pause request
  
    form.querySelectorAll("input, button, select, textarea").forEach(ctrl => {
            ctrl.disabled = true;
    });

    try {
        const blob = await getBlob();       // your existing function
        input.value = await blobToBase64(blob);

        form.querySelectorAll("input, button, select, textarea").forEach(ctrl => {
            ctrl.disabled = false;
        });

        e.detail.issueRequest(true);        // continue with the request
    }
    catch {
        form.querySelectorAll("input, button, select, textarea").forEach(ctrl => {
            ctrl.disabled = false;
        });
    }
});

async function sha256Hex(str) {
    const data = new TextEncoder().encode(str);
    const hash = await crypto.subtle.digest('SHA-256', data);
    return Array.from(new Uint8Array(hash)).map(b => b.toString(16).padStart(2, '0')).join('');
}

async function getBlob() {
    const iframe = document.getElementById('iframeView').contentWindow.document.getElementById('iframeView') ?? document.getElementById('iframeView');
    const doc = iframe ? iframe.contentWindow.document : document;

    await convertRelativeImagesToDataURI(doc);

    const html = iframe ? doc.documentElement.outerHTML : document.getElementById('printable-content').innerHTML;

    // If Electron path, do not use cache at all.
    if (window.electronAPI?.printToPDF) {
        const pdfData = await window.electronAPI.printToPDF({ html, pdfOptions: { printBackground: true, format: 'a4', preferCSSPageSize: true } });
        return new Blob([pdfData], { type: 'application/pdf' });
    }

    // Network path: check cache BEFORE contacting server.
    const key = await sha256Hex(html);
    const cacheUrl = `/${key}.pdf`;

    let cacheObj;
    if ('caches' in self) cacheObj = await caches.open('pdf-cache');

    if (cacheObj) {
        const cached = await cacheObj.match(cacheUrl);
        if (cached) return cached.blob();
    }

    // Miss: call server, then store.
    const formData = new FormData();
    formData.append('Html', html);

    const res = await fetch('/pdf', { method: 'POST', body: formData });
    if (!res.ok) {
        const text = await res.text();
        const errorRedirect = res.headers.get('Error-Redirect');
        showError(text, errorRedirect);
        throw Error(text);
    }

    const contentType = res.headers.get('Content-Type');
    if (!contentType || !contentType.includes('application/pdf')) {
        const text = await res.text();
        const errorRedirect = res.headers.get('Error-Redirect');
        showError(text, errorRedirect);
        throw Error(text);
    }

    const blob = await res.blob();

    if (cacheObj) {
        try {
            await cacheObj.put(cacheUrl, new Response(blob));
        } catch { } // ignore quota/eviction issues
    }

    return blob;
}

async function getPdf(e, filename) {
    e.disabled = true;
    try {
        const blob = await getBlob();
        const url = URL.createObjectURL(blob);

        const a = document.createElement('a');
        a.href = url;
        a.download = filename;
        a.click();

        URL.revokeObjectURL(url);
    }
    finally {
        e.disabled = false;
    }
}

function blobToBase64(blob) {
  return new Promise((resolve, reject) => {
    const r = new FileReader();
    r.onloadend = () => {
      const dataUrl = r.result;                     // "data:application/pdf;base64,...."
      resolve(dataUrl.split(',')[1]);               // base64 only
    };
    r.onerror = reject;
    r.readAsDataURL(blob);
  });
}

async function convertRelativeImagesToDataURI(doc) {
    const images = doc.querySelectorAll('img');

    await Promise.all(Array.from(images).map(async img => {
        const src = img.getAttribute('src');

        if (src && !/^(https?:)?\/\//i.test(src)) {
            try {
                const response = await fetch(src);
                const blob = await response.blob();
                const reader = new FileReader();

                const dataUri = await new Promise((resolve, reject) => {
                    reader.onloadend = () => resolve(reader.result);
                    reader.onerror = reject;
                    reader.readAsDataURL(blob);
                });

                img.src = dataUri;

            } catch (err) {
                console.error(`Failed to inline image ${src}:`, err);
            }
        }
    }));
}