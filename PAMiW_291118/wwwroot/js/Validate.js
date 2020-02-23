function getMethod() {
    axios.get('/api/Api/GetToken')
        .then(function(response) {
            var token = "Bearer " + response.data;
            axios.get('http://localhost:8081/api/Api/Values', { params: {}, headers: { 'Authorization': token } })
                .then(function(response) {
                    // handle success
                    console.log(response);
                })
                .catch(function(error) {
                    // handle error
                    console.log(error);
                })
            //$.ajax({
            //    url: 'http://localhost:62181/api/Api/Values',
            //    type: 'GET',
            //    dataType: 'json',
            //    headers: {
            //        'Authorization': token,
            //        'Access-Control-Allow-Origin': '*',
            //        'Access-Control-Allow-Methods': 'GET',
            //        'Access-Control-Allow-Headers': '*'
            //    },
            //    success: function(response) {
            //        console.log(response);
            //    },
            //    error: function(response) {
            //        console.log(response);
            //    }
            //}).done(function() {
            //});
        })
        .catch(function(error) {
            // handle error
            console.log(error);
        });
}