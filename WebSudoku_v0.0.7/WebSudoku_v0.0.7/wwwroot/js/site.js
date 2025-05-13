// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

window.onload = function () {
    this.getAllPuzzles();
};

function getAllPuzzles() {
    fetch("api/sudoku/getallpuzzles")
        .then(res => res.json())
        .then((rawData) => {
            var data = translateResponseData(rawData);
            hydrateSelectElem(data);
        })
        .catch(err => console.log(err));
}

function translateResponseData(puzzles) {
    var data = JSON.parse(puzzles);
    return data;
}

function hydrateSelectElem(puzzle) {
    var select = document.getElementById("puzzleSelect");
    select.innerHTML = "";
    var puzzleInnerHTML = select.innerHTML;

    for (var i = 0; i < puzzle.length; i++) {
        puzzleInnerHTML += "<option>" + puzzle[i].boardValues + "</option>\r\n";
    }
    select.innerHTML = puzzleInnerHTML;
}