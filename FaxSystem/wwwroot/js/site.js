document.querySelectorAll('input[required]').forEach((input) => {
    input.addEventListener('invalid', function (event) {
        if (event.target.validity.valueMissing) {
          event.target.setCustomValidity('برجاء ادخال البيانات');
        }
      });
      
      input.addEventListener('change', function (event) {
        event.target.setCustomValidity('');
      });
})