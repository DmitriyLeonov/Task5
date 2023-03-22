let formData = {};
const form = document.querySelector('form');
const ls = localStorage;

form.addEventListener('input', function(event) {
    formData[event.target.name] = event.target.value;
    ls.setItem('formData', JSON.stringify(formData));
});

if (ls.getItem('formData')) {
    formData = JSON.parse(ls.getItem('formData'));
    for (let key in formData) {
        form.elements[key].value = formData[key];
    }
}
ls.clear();
