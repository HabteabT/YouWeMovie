document.addEventListener('DOMContentLoaded', function () {
    const elements = document.querySelectorAll('.BB');

    elements.forEach((element, index) => {
        element.addEventListener('mouseover', () => {
            // Add 'hovered' class to the current and preceding elements
            for (let i = index; i >= 0; i--) {
                elements[i].classList.add('hovered');
            }
        });

        element.addEventListener('mouseout', () => {
            // Remove 'hovered' class from all elements
            elements.forEach((el) => {
                el.classList.remove('hovered');
            });
        });
    });
});


function openDialog() {
    document.querySelector('.dialog-overlay').style.display = 'flex';
}

function closeDialog() {
    document.querySelector('.dialog-overlay').style.display = 'none';
}


document.getElementById("filterSelect").addEventListener("change", function () {
    var selectedFilter = this.value;
    var baseUrl = window.location.href.split('?')[0];
    window.location.href = baseUrl + '?filter=' + selectedFilter;
});