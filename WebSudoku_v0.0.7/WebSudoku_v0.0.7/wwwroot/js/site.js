// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

window.onload = function () {
    this.getAllPuzzles();
    //  MOCK DATA inline testing
    //var json = "{\"boardValues\":\"010096830930200040824703100005900000600085429208004061700030080000800003001049207\", \"id\":0, \"difficulty\":0}";
    //this.addPuzzle(JSON.parse(json));
    //this.deletePuzzle("1EE9E35B-FB51-4BBB-914B-11EA33846E2B");
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

function hydrateSelectElem(puzzle) {
    var select = document.getElementById("puzzleSelect");
    select.innerHTML = "";
    var puzzleInnerHTML = select.innerHTML;

    for (var i = 0; i < puzzle.length; i++) {
        puzzleInnerHTML += "<option>" + puzzle[i].boardValues + "</option>\r\n";
    }
    select.innerHTML = puzzleInnerHTML;
}