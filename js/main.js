document.getElementById('date').innerHTML = new Date().toDateString();
document.getElementById('logButton').onclick = validate;


function validate() {
    //allows only Winthrop emails for login
    var emailFormat = /^[a-zA-Z0-9_.+-]+@(?:(?:[a-zA-Z0-9-]+)?[a-zA-Z]+)?(winthrop|mailbox.winthrop)\.edu$/;
    var password = document.getElementById('passBox').value;
    var email = document.getElementById('emailBox').value;

    if (email.match(emailFormat))
    {
        console.log("Email is good");
    } else {
        console.log("Email is bad");

    }
    
}