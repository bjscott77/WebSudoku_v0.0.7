// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

window.onload = function () {
    this.getAllPuzzles();
    //var puzzle = "{\"boardValues\": \"568000019000000000302915080006071543800040600403056098704008261600000000019004035\", \"difficulty\": 0, \"id\": 0 }"; 
    //var jsonPuzzle = JSON.parse(puzzle);
    //this.addPuzzle(jsonPuzzle);
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


function addPuzzle(puzzle) {
    fetch("api/sudoku/addpuzzle/", {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'            
        },
        body: JSON.stringify(puzzle)
    })
        .then(response => {
            response.json();
        })
        .then(data => {
            console.log('Success:', data);
        })
        .catch(error => console.error('Error:', error));
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