
fetch("http://localhost:5000/", {
    method: "CHECK"
}).then(out => out.text()).then(data => {
    console.log(data);
})
Redirect();

const input = document.getElementById("in");

function Redirect() {
    console.log("TRIED");
        fetch("http://localhost:5000/", {
            method: "AUTH",
            body: JSON.stringify({
                authKey: localStorage.getItem("authKey")
            })
        }).then(out => out.text()).then(data => {
            console.log(data);
            if(data == "AUTHORIZED") {
                console.log(localStorage.getItem("authKey"));
                console.log(data);
                window.location.href = "page.html";
            }
        })

}

function CheckPass() {
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