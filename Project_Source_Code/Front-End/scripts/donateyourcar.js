function createDonateYourCarView(){
    fetchWithTokens('http://localhost:5212/DonateYourCar/GetCharities','POST','')
    .then(function(response){
        if(response.status == 200){
            response.json()
            .then(function(data){
                
            })
        }

    })
    
}