function ScrollButtons(containerClass){
    //container class is the name of the container that holds everything inside it
    const sectionContainers = document.querySelectorAll("." + containerClass);
    
    sectionContainers.forEach((container) => {
        const imageContainer = container.querySelector('.image-container');
        const prevButton = container.querySelector('.prev-button');
        const nextButton = container.querySelector('.next-button');
    
        let currentImageIndex = 0;
    
        function showImage(index) {
            // 125.5 shows exactly 5 pictures at a time in the image container
            const offset = -index * 125.5;
            imageContainer.style.transform = `translateX(${offset}%)`; 
        }
    
        function prevImage(numOfContents) {
            // gets the number of clicks before going back around
            let numOfClicks = Math.ceil((Number(numOfContents)/5));
            
            // modulo the (current index + number of clicks -1) with number of clicks, to get the previous index
            currentImageIndex = (currentImageIndex + numOfClicks - 1) % numOfClicks;
            showImage(currentImageIndex);
        }
    
        function nextImage(numOfContents) {
            // gets the number of clicks before going back around
            let numOfClicks = Math.ceil((Number(numOfContents)/5));
            
            // modulo the current index + 1 with number of clicks will return the index to 0 once required clicks is made.
            // +1 makes it go to next slide
            currentImageIndex = (currentImageIndex + 1) % numOfClicks;
            console.log((Number(numOfContents)/5))
            showImage(currentImageIndex);
        }
    
        prevButton.addEventListener('click', () => prevImage(prevButton.dataset.contents));
        nextButton.addEventListener('click', () => nextImage(prevButton.dataset.contents));
    });
}