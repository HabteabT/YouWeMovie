function updateList(event){
    event.preventDefault(); //Prevents the default form submission
    
    let form = $(event.target)
    let formData = form.serialize();
    let button = form.find('button[type="submit"]');
    
    $.ajax({
        url: form.attr('action'), // Gets the action attribute from the form
        type: 'POST', // Type of request
        data: formData, // Data to be sent
        success: function(response){
                if(response.success){
                    //if the content is successfully added to the users list, it will update the button accordingly
                    // if the length is higher than 12 it means the button is already displaying "Remove from list"
                    if(button.html().includes("Remove")){
                        button.html( "Add to list");
                    } else 
                        button.html( "Remove from list");
                } 
                
                else {
                    alert("error");
                }            
            },
        error: function(xhr, status, error){
            console.error("Error in AJAX request: " + error);
        }
    });
}
