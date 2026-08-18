document.getElementById('search').addEventListener('input', updateValue);
document.getElementById('search').addEventListener('click', updateValue);

function updateValue(e) {
	document.getElementById('search').classList.remove('text-neutral-200');
	var text = document.getElementById('search').value;
	var businesses = document.getElementsByClassName('business');
	var any = false;
	for (i = 0; i < businesses.length; i++) {
		var business = businesses[i];
		var name = business.children[1].children[0].innerText;
		if (text == null || text.length == 0 || name.toLowerCase().includes(text.toLowerCase())) {
			any = true;
			business.classList.remove('hidden');
		}
		else {
			business.classList.add('hidden');
		}
	}
	if (!any) {
		document.getElementById('search').classList.add('text-red-600');
	}
	else {
		document.getElementById('search').classList.remove('text-red-600');
	}
}