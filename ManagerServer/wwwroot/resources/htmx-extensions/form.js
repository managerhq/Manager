document.addEventListener("htmx:configRequest", async function(e) {
    const fd = e.detail.formData || e.detail.parameters;
    const magicString = 'febb4049-dcdb-4c7a-a395-4b71da72a85b';
    if (fd.has(magicString)) {
        fd.set(magicString, JSON.stringify(app.$data));
        
        const fileInput = document.getElementById('image-input');
        if (fileInput?.files?.length) fd.set('Image', fileInput.files[0]);
        if (document.getElementById('ImageDeleted') != null) fd.set('ImageDeleted', document.getElementById('ImageDeleted').value);
    }
});