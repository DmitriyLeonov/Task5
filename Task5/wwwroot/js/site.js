    let formData = {};
    const form = document.querySelector("form");
    const ls = sessionStorage;
    

form.addEventListener("change", function (event) {
        formData[event.target.name] = event.target.value;
        ls.setItem("formData", JSON.stringify(formData));
    });

    if (ls.getItem("formData")) {
        formData = JSON.parse(ls.getItem("formData"));
        console.log(form.elements);
        for (let key in formData) {
            if (Object.prototype.hasOwnProperty.call(formData, key)) {
                form.elements[key].value = formData[key];
            }
        }
    }
    ls.clear();

