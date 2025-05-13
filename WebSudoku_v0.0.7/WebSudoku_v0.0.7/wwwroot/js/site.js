// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var selectedPuzzle;
window.onload = function () {
    this.getAllPuzzles();
};

function getAllPuzzles() {
    fetch("api/sudoku/getallpuzzles")
        .then(res => res.json())
        .then((rawData) => {
            var data = translateResponseData(rawData);
            data.selectedPuzzle = selectedPuzzle;
            hydrateSelectElem(data);
            hydrateRootElem(data);
        })
        .catch(err => console.log(err));
}

function addPuzzle(puzzle) {
    selectedPuzzle = puzzle;
    fetch("api/sudoku/addpuzzle", {
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

function deletePuzzle(id) {
    fetch("api/sudoku/deletepuzzle", {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(id)
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

function hydrateSelectElem(puzzles) {
    var select = document.getElementById("puzzleSelect");

    select.innerHTML = "";
    var puzzleInnerHTML = select.innerHTML;

    if (puzzles.includes(puzzles.selectedPuzzle))
        puzzles = swapArrayElements(puzzles, 0, puzzles.indexOf(puzzles.selectedPuzzle));

    for (var i = 0; i < puzzles.length; i++) {
        puzzleInnerHTML += "<option>" + puzzles[i].boardValues + "</option>\r\n";
    }
    select.innerHTML = puzzleInnerHTML;
}

function hydrateRootElem(puzzles) {
    var select = document.getElementById("puzzleSelect");
    var root = document.getElementById("root");
    var rootInnerHTML = "";
    var puzzle = puzzles[0];

    for (var i = 0; i < puzzle.boardValues.length; i++) {
        rootInnerHTML += "<div class='cell'>" + puzzle.boardValues[i] + "</div>";
    }
    root.innerHTML = rootInnerHTML;
}

function swapArrayElements(array, index1, index2) {
    [array[index1], array[index2]] = [array[index2], array[index1]];
    return array;
};