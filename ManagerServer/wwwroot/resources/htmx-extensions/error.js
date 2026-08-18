document.addEventListener('htmx:responseError', (event) => {
	showError(event.detail.xhr.responseText);
});

function showError(text, errorRedirect) {
	const dlg = document.getElementById('error-dialog');
	const msg = document.getElementById('error-message');
	msg.textContent = text;
	dlg.showModal();

	if (errorRedirect) {
		dlg.addEventListener('close', () => {
			window.location.href = errorRedirect;
		}, { once: true });
	}
};