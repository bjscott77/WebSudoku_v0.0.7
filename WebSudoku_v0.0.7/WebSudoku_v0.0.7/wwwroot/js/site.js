// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
window.onload = function () {
    this.getAllPuzzles();

    let selectElem = document.getElementById("puzzleSelect");
    selectElem.addEventListener("change", function (event) {
        var data = JSON.parse("[{\"boardValues\": \"" + event.target.value + "\", \"id\":0, \"difficulty\":0}]");
        hydrateRootElem(data);
    });
};

function getAllPuzzles() {
    fetch("api/sudoku/getallpuzzles")
        .then(res => res.json())
        .then((rawData) => {
            var data = translateResponseData(rawData);
            hydrateSelectElem(data);
            hydrateRootElem(data);
        })
        .catch(err => console.log(err));
}

function addPuzzle(puzzle) {
    fetch("api/sudoku/addpuzzle", {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'            
        },
        body: JSON.stringify(puzzle)
    })
        .then(response => {
            return response.json();
        })
        .then(rawData => {
            var data = translateResponseData(rawData);
            if (data.StatusCode == "409") {
                alert(data.Status + ": " + data.ErrorMessage);
            } else if (data.StatusCode == "200") {
                hydrateSelectElem(data);
            }
            
        })
        .catch(error => console.error('Error:', error));
}

function solvePuzzle(puzzle) {
    fetch("api/sudoku/getsolvedpuzzle?puzzle="+puzzle)
        .then(res => res.json())
        .then((rawData) => {
            var data = translateResponseData(rawData);
            hydrateSelectElem(data);
            hydrateRootElem(data);
        })
        .catch(err => console.log(err));
}

function prepareAddNewPuzzle() {
    var puzzle = document.getElementById("newPuzzleInput");
    if (puzzle.value == null || puzzle.value == "" || puzzle.value == 'undefined') {
        alert("Please enter a puzzle to save it.");
        return;
    }
    var jsonPuzzle = JSON.parse("{\"boardValues\":\"" + puzzle.value + "\",\"difficulty\":0, \"id\":0 }");
    addPuzzle(jsonPuzzle);
}

var even = false;
function toggleNewPuzzleDisplay() {
    var elem = document.getElementById("addToggle");
    var button = document.getElementById("addNew");

    if (even) {
        button.innerHTML = "Add New..."
        elem.style.display = 'none';

    } else {
        button.innerHTML = "Back..."
        elem.style.display = 'block';        
    }
    even = !even;
}

function deletePuzzle(puzzle) {
    fetch("api/sudoku/deletepuzzle", {
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

function deleteSelectedPuzzle() {   
    var selectElem = document.getElementById("puzzleSelect");
    this.deletePuzzle(selectElem.value);
}

function activateGameSolution() {
    var selectElem = document.getElementById("puzzleSelect");
    var puzzle = selectElem.value.toString();
    this.solvePuzzle(puzzle);
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
    var select = document.getElementById("puzzleSelect").value;
    var root = document.getElementById("root");
    var rootInnerHTML = "";
    var puzzle = puzzles[0];

    for (var i = 0; i < select.length; i++) {
        rootInnerHTML += "<div class='cell'>" + select[i] + "</div>";
    }
    root.innerHTML = rootInnerHTML;
}

function swapArrayElements(array, index1, index2) {
    [array[index1], array[index2]] = [array[index2], array[index1]];
    return array;
};