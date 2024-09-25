$(document).ready(function () {
    // Setter filteret til høyeste og alle content når siden blir lastet inn, og om det er en serie eller en film
    var theFilterType = 'Highest';
    var theGenre = 'All';
    //sjekker om det er en serie eller film
    var IsSeries = new URLSearchParams(window.location.search).get('IsSeries') === 'true';
    getContent(theFilterType, theGenre, IsSeries);

    // En event listner som bruker JQuery, som venter på at movie knappen skal bli trykket på
    $('#moviesButton').click(function () {
        IsSeries = false; //fordi det er snakk om filmer
        getContent(theFilterType, theGenre, IsSeries);
    });

    // En event listner som bruker JQuery, som venter på at series knappen skal bli trykket på
    $('#seriesButton').click(function () {
        IsSeries = true; //foedi det er snakk om serier
        getContent(theFilterType, theGenre, IsSeries);
    });

    // En event listner som bruker JQuery, som venter på en forandring i filteret
    $('.filter-select').on('change', function () {
        //Finner et element med klasse filter-type
        theFilterType = $('.filter-type').val();
        //Finner et element med klasse genre-type
        theGenre = $('.genre-type').val();
        //bruker getContent med de 3 nye argumentene
        getContent(theFilterType, theGenre, IsSeries);
    });
    
    function getContent(filterType, genre, IsSeries) {
        $.ajax({ //Henter en HHTP Ajax request
            // Requesten blir sendt til følgende URL
            url: '/Content/GetContent',
            //Requesten blir sendt som en POST 
            method: 'POST',
            // Det blir sendt som en JSON 
            contentType: 'application/json',
            // Konverterer content til JSON ved bruk av JSON.stringify
            data: JSON.stringify({filterType: filterType, genre: genre, IsSeries: IsSeries}),
            success: function (response) {
                //Når Ajax requesten er sucsessfull, sender HTML-en inn i movie-container
                $('.movie-container').html(response.html);
                //PÅ denne måten vil content endre seg fortlåpende etter filter og genre
            }
        });
    }

    //Initialiserer getContent når siden lastes inn
    getContent(theFilterType, theGenre, IsSeries);
});



