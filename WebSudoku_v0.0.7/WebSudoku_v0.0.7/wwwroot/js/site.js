// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

window.onload = function () {
    fetch("api/sudoku/getallpuzzles")
        .then(res => res.json())
        .then((rawData) => {
            hydrateSelectElem(translateResponseData(rawData));
        })
        .catch(err => console.log(err));
};

function translateResponseData(rawData) {
    var data = [""];
    var splitData = rawData.toString().split(",");
    for (var i = 0; i < splitData.length; i++) {
        data[i] = splitData[i].replace("[\"", "").replace("\"", "").replace("\"]", "").replace("\"", "")
    }
    return data;
}

function hydrateSelectElem(data) {
    var select = document.getElementById("puzzleSelect");
    select.innerHTML = "";
    var puzzleInnerHTML = select.innerHTML;

    for (var i = 0; i < data.length; i++) {
        puzzleInnerHTML += "<option>" + data[i] + "</option>\r\n";
    }
    select.innerHTML = puzzleInnerHTML;
}