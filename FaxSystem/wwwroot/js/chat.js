//"use strict";

//var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

////Disable the send button until connection is established.
//document.getElementById("sendButtonfax").disabled = true;

////connection.on("ReceiveMessage", function (user, message) {
////    var li = document.createElement("li");
////    document.getElementById("messagesList").appendChild(li);
////    // We can assign user-supplied strings to an element's textContent because it
////    // is not interpreted as markup. If you're assigning in any other way, you
////    // should be aware of possible script injection concerns.
////    li.textContent = `${user} says ${message}`;
////});

//connection.start().then(function () {
//    document.getElementById("sendButtonfax").disabled = false;
//}).catch(function (err) {
//    return console.error(err.toString());
//});

//document.getElementById("sendButtonfax").addEventListener("click", function (event) {
//  //  var user = document.getElementById("userInput").value;
//   // var message = document.getElementById("messageInput").value;

//    connection.invoke("SendMessage", "11","11").catch(function (err) {
//        return console.error(err.toString());
//    });
//    event.preventDefault();
//});

"use strict";

var connection;

    connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();


  

connection.start().then(function () {
    
}).catch(function (err) {
        return console.error(err.toString());
    });
function send(b) {
    connection.start().then(function () {
    if (b != -1) {

        connection.invoke("SendMessage", "11", "11").catch(function (err) {
            return console.error(err.toString());
        });
        }
    }).catch(function (err) {
        return console.error(err.toString());
    });
}
function rec() {
    connection.on("ReceiveMessage", function (user, message) {

        $.ajax({
            type: "POST",
            url: "/Home/numofsus",
            data: {},
            success: function (data) {
                console.log(data);
                var a = data;
                document.getElementById("NumberOfSuspendedFaxesid").innerText = a;
            }
        });
    });
}