class SearchBar{

    // 0-3 , all, movies, series, users
    category = 0;
    suggestionBox = document.getElementById('suggestions');
    setCategory(){
        this.category = Number(document.getElementById("search-category").value);
    }
    search()
    {
        let input = document.getElementById('search-field').value;

        // to not spam database request, its updated each second character added (might be changed)
        if (input.length > 0 &&
            input.length % 2 === 0){
            // sending request to the controller using AJAX
            fetch('/Search/Search?input=' + input + '&cat=' + this.category)
                .then(response => response.json())
                .then(data => {


                    //removes divs from last request:
                    this.suggestionBox.innerHTML = '';

                    if (this.category === 0){
                        // lists out all the users
                        data.users.result.forEach(item => {
                            this.addPartial("/Search/UserResult", item);
                        });
                        //lists out all the contents
                        data.contents.result.forEach(item => {
                            this.addPartial("/Search/ContentResult", item)
                        });
                        //for users
                    }else if (this.category === 3) {
                        data.result.forEach(item => {
                            this.addPartial("/Search/UserResult", item);
                        });
                        // for contents
                    }else
                        data.result.forEach(item => {
                            this.addPartial("/Search/ContentResult", item)
                        });
                    // makes box visaple
                    this.suggestionBox.style.display = 'block';
                });
        }else if (input.length <=0)
            //hides it
            this.suggestionBox.style.display = 'none'
    }

    //function that displays partial view.
    addPartial(url, model){
        if(model == null)
            return;

        fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(model)
        })
            .then(response => response.text())
            .then(html => {
                this.suggestionBox.innerHTML += html;
            });
    }
}

let searchBar = new SearchBar;