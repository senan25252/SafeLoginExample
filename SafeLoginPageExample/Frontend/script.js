
fetch("http://localhost:5000/", {
    method: "CHECK"
}).then(out => out.text()).then(data => {
    console.log(data);
})
Redirect();


function Redirect() {
    console.log("TRIED");
    fetch("http://localhost:5000/", {
        method: "AUTH",
        body: JSON.stringify({
            authKey: localStorage.getItem("authKey")
        })
    }).then(out => out.text()).then(data => {
        if(data != "") {
        console.log(data);
        document.body.innerHTML = data;
        }
    })

}

function CheckPass() {
    
    const input = document.getElementById("in");
    fetch("http://localhost:5000/", {
        method: "POST",
        body: JSON.stringify({
            passkey: input.value
        })
    }).then(out => out.text()).then(data => {
        localStorage.setItem("authKey", data);
        console.log(data);
        Redirect();
    })

}