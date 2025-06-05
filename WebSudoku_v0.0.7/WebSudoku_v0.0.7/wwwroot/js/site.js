window.onload = function () {
    this.getAllPuzzles();

    const selectElem = document.getElementById("puzzleSelect");
    selectElem.addEventListener("change", function (event) {
        getPuzzle();
    });

    const AddElem = document.getElementById("newPuzzleInput");
    AddElem.addEventListener("focus", function (event) {
        let btn = document.getElementById("addNew");
        btn.innerHTML = "Add";
    });

    const AddRatingElem = document.getElementById("newPuzzleRating");
    AddRatingElem.addEventListener("focus", function (event) {
        let btn = document.getElementById("addNew");
        btn.innerHTML = "Add";
    });

    const updateElem = document.getElementById("updatePuzzleInput");
    updateElem.addEventListener("focus", function (event) {
        let btn = document.getElementById("updatePuzzle");
        btn.innerHTML = "Update";
    });

    const updateRatingElem = document.getElementById("updatePuzzleRating");
    updateRatingElem.addEventListener("focus", function (event) {
        let btn = document.getElementById("updatePuzzle");
        btn.innerHTML = "Update";
    });     
};

function getAllPuzzles() {
    fetch("api/sudoku/getallpuzzles")
        .then(res => res.json())
        .then((rawData) => {
            const data = translateResponseData(rawData);
            if (data.StatusCode == 200) 
                hydrateAll(data);
            else
                modal(data.Status + ": " + data.ErrorMessage);
            
        })
        .catch(err => {
            console.log(err);
        });
}

function getPuzzle(puzzle) {
    const selectElem = document.getElementById("puzzleSelect");
    const id = selectElem.value;

    if (puzzle == null || puzzle == "" || puzzle == 'undefined') {
        for (let i = 0; i < selectElem.length; i++) {
            if (selectElem[i].value == id)
                puzzle = selectElem[i].innerHTML;
        }
    }

    if (puzzle == null) {
        modal("Please select a puzzle to load it.");
    } else {
        fetch("api/sudoku/getpuzzle?puzzle=" + puzzle + "&id=" + id)
            .then(res => res.json())
            .then((rawData) => {
                const data = translateResponseData(rawData);
                if (data.StatusCode == 200) {
                    hydrateRootElem(data);
                    hydrateRatingElem(data);
                } else {
                    modal(data.Status + ": " + data.ErrorMessage);
                }
            })
            .catch(err => console.log(err));
    }

}

function addPuzzle() {
    const puzzle = document.getElementById("newPuzzleInput");
    const rating = document.getElementById("newPuzzleRating");

    if ((puzzle.value == null && rating.value == null) || (puzzle.value == "" && rating.value == "") || (puzzle.value == 'undefined' && rating.value == 'undefined')) {
        modal("Please enter a puzzle and rating to save it.");
        return;
    }
    if (rating.value == null || rating.value == "" || rating.value == 'undefined')
        rating.value = 0;

    const newObj = { boardValues: puzzle.value, id: 0, difficulty: +rating.value };

    fetch("api/sudoku/addpuzzle", {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(newObj)
    })
        .then(response => {
            return response.json();
        })
        .then(rawData => {
            const data = translateResponseData(rawData);
            if (data.StatusCode == "200") 
                hydrateAll(data);   
            else 
                modal(data.Status + ": " + data.ErrorMessage);       

        })
        .catch(error => console.error('Error:', error));
}

function updatePuzzle() {
    const uPuzzle = document.getElementById("updatePuzzleInput");
    const sPuzzle = document.getElementById("puzzleSelect");
    const uRating = document.getElementById("updatePuzzleRating");
    const sRating = document.getElementById("selectPuzzleRating");

    if ((uPuzzle.value == null && uRating.value == null) || (uPuzzle.value == "" && uRating.value == "") || (uPuzzle.value == 'undefined' && uRating.value == 'undefined')) {
        modal("Please select a puzzle to update it.");
        return;
    }

    if (uPuzzle.value == sPuzzle.value && uRating.value == sRating.value) {
        modal("No changes were specified, so no updates were made.");
        return;
    }

    if (uRating.value == null || uRating.value == "" || uRating.value == 'undefined')
        uRating.value = 0;

    const putObj = [{ boardValues: sPuzzle.value, id: 0, difficulty: 0 }, { boardValues: uPuzzle.value, id: 0, difficulty: +uRating.value }];

    fetch("api/sudoku/updatepuzzle", {
        method: 'PUT',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(putObj)
    })
        .then(response => {
            return response.json();
        })
        .then(rawData => {
            const data = translateResponseData(rawData);
            if (data.StatusCode == 200) {
                hydrateRootElem(data)
                data.Payload = data.Payload.slice(1, data.length);
                hydrateSelectElem(data);
                hydrateRatingElem(data);
            } else {
                modal(data.Status + ": " + data.ErrorMessage);
            }

        })
        .catch(error => console.error('Error:', error));
}

function solvePuzzle() {
    disableAll();
    let puzzle = getCurrentPuzzle();
    console.log("Solve Puzzle", puzzle);

    if (puzzle == null) {
        modal("Please select a puzzle to solve it.");
    } else {
        fetch("api/sudoku/getsolvedpuzzle?puzzle=" + puzzle)
            .then(res => res.json())
            .then((rawData) => {
                const data = translateResponseData(rawData);
                if (data.StatusCode == 200)
                    hydrateRootElem(data);
                else
                    modal(data.Status + ": " + data.ErrorMessage);

                enableAll();
            })
            .catch(err => console.log(err));
    }
}

function stepPuzzle() {
    modal("Not Ready: Single step solution is under construction.");
}

function confirmDelete() {
    const puzzle = document.getElementById("puzzleSelect")?.value;

    if (puzzle == null || puzzle == "" || puzzle == 'undefined') {
        modal("Please select a puzzle to delete it.");
    } else {
        modalConfirm("Are you sure you want to delete?")
            .then(confirmed => {
                if (confirmed) {
                    deletePuzzle();
                } else {
                    modal("Delete was cancelled, no action taken.");
                }
            })
            .catch(error => {
                console.log(error);
            });
    }
}

function deletePuzzle() {
    const puzzle = document.getElementById("puzzleSelect")?.value;

    if (puzzle == null || puzzle == "" || puzzle == 'undefined') {
        modal("Please select a puzzle to delete it.");
    } else {
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
                if (data.StatusCode == 200)
                    modal('Puzzle deleted successfully.');
                else
                    modal(data.Status + ": " + data.ErrorMessage);
            })
            .catch(error => console.error('Error:', error));
    }
}

let toggleNew = true;
function toggleNewPuzzleForm() {
    const elem = document.getElementById("addToggle");
    const button = document.getElementById("addNew");
    const updatebtn = document.getElementById("updatePuzzle");
    const deletebtn = document.getElementById("deletePuzzle");
    const solvebtn = document.getElementById("solvePuzzle");
    const resetbtn = document.getElementById("resetPuzzle");

    if (toggleNew) {
        button.innerHTML = "Back"
        elem.style.display = 'block';
        updatebtn.style.display = 'none';
        deletebtn.style.display = 'none';
        solvebtn.style.display = 'none';
        resetbtn.style.display = 'none';
    } else {
        const newPuzzle = document.getElementById("newPuzzleInput")?.value;

        elem.style.display = 'none';
        updatebtn.style.display = 'block';
        deletebtn.style.display = 'block';
        solvebtn.style.display = 'block';
        resetbtn.style.display = 'block';

        if (newPuzzle == null || newPuzzle == "" || newPuzzle == 'undefined') {
            if (button.innerHTML == "Add")
                modal("No puzzle was entered, so no new puzzles were added.");
        } else {
            this.addPuzzle();
        }
        button.innerHTML = "Add..."
    }
    toggleNew = !toggleNew;
}

let toggleUpdate = true;
function toggleUpdatePuzzleForm() {
    const elem = document.getElementById("updateToggle");
    const button = document.getElementById("updatePuzzle");
    const addbtn = document.getElementById("addNew");
    const deletebtn = document.getElementById("deletePuzzle");
    const solvebtn = document.getElementById("solvePuzzle");
    const resetbtn = document.getElementById("resetPuzzle");
    const selectPuzzle = document.getElementById("puzzleSelect");
    let updatePuzzle = document.getElementById("updatePuzzleInput");
    const rating = document.getElementById("selectPuzzleRating");
    const updateRating = document.getElementById("updatePuzzleRating");
    let selectValue = "";

    for (let i = 0; i < selectPuzzle.length; i++) {
        if (selectPuzzle[i].value == selectPuzzle.value) {
            updatePuzzle.value = selectPuzzle[i].innerHTML;
            selectValue = selectPuzzle[i].innerHTML;
            break;
        }
    }
    if (toggleUpdate) {
        button.innerHTML = "Back"
        elem.style.display = 'block';
        addbtn.style.display = 'none';
        deletebtn.style.display = 'none';
        solvebtn.style.display = 'none';
        resetbtn.style.display = 'none';
    } else {
        updatePuzzle = document.getElementById("updatePuzzleInput");
        if (updatePuzzle.innerHTML == null || updatePuzzle.innerHTML == "" || updatePuzzle.innerHTML == 'undefined') {
            if (button.innerHTML == "Update")
                modal("No puzzle was entered, so no puzzles were updated.");
        } else if (updatePuzzle.innerHTML == selectValue && updateRating.innerHTML == rating.value) {
            if (button.innerHTML == "Update")
                modal("No changes were made, so no puzzles were updated.");
        } else {
            this.updatePuzzle();
        }
        button.innerHTML = "Update..."
        elem.style.display = 'none';
        addbtn.style.display = 'block';
        deletebtn.style.display = 'block';
        solvebtn.style.display = 'block';
        resetbtn.style.display = 'block';
    }
    toggleUpdate = !toggleUpdate;
}   

let toggleSolve = true;
function toggleSolvePuzzleForm() {
    const solveToggle = document.getElementById("solveToggle");
    const solvePuzzle = document.getElementById("solvePuzzle");
    const addbtn = document.getElementById("addNew");
    const updatebtn = document.getElementById("updatePuzzle");
    const deletebtn = document.getElementById("deletePuzzle");

    if (toggleSolve) {
        solvePuzzle.innerHTML = "Back";
        solveToggle.style.display = 'block';
        addbtn.style.display = 'none';
        updatebtn.style.display = 'none';
        deletebtn.style.display = 'none';
    } else {
        solvePuzzle.innerHTML = "Solve...";
        solveToggle.style.display = 'none';
        addbtn.style.display = 'block';
        updatebtn.style.display = 'block';
        deletebtn.style.display = 'block';
    }
    toggleSolve = !toggleSolve;
}

function translateResponseData(puzzles) {
    const data = JSON.parse(puzzles);
    return data;
}

function hydrateAll(puzzles) {
    hydrateSelectElem(puzzles);
    hydrateRatingElem(puzzles);
    hydrateRootElem(puzzles);
}

function hydrateRatingElem(puzzles) {
    const rating = document.getElementById("selectPuzzleRating");
    rating.value = puzzles.Payload[0].difficulty;
}

function hydrateSelectElem(puzzles) {
    const select = document.getElementById("puzzleSelect");
    select.innerHTML = "";

    for (let i = 0; i < puzzles.Payload.length; i++) {
        select.innerHTML += `<option value='${puzzles.Payload[i].Id}'>${puzzles.Payload[i].boardValues}</option>\r\n`;
    }
}

function hydrateRootElem(puzzles) {
    const select = document.getElementById("puzzleSelect").value;
    const root = document.getElementById("root");
    let rootInnerHTML = "";
    const puzzle = puzzles.Payload[0].boardValues;

    for (let i = 0; i < puzzle.length; i++) {
        let possible = puzzles.Payload[0].possibles[i] != undefined ? puzzles.Payload[0].possibles[i] : "";
        if (possible == "") {
            rootInnerHTML += `<div class="cell">${puzzle[i]}</div>`;
        } else {
            rootInnerHTML += `<div class="cell" data-title=${possible}>${puzzle[i]}</div>`;
        }
    }

    if (puzzles.CellDisplayValueType == "SPACE") {
        rootInnerHTML = rootInnerHTML.replaceAll("0", "&nbsp");
    }

    root.innerHTML = rootInnerHTML;

    for (let i = 0; i < root.children.length; i++) {
        root.children[i].addEventListener("click", function (event) {
            let value = 0;
            if (event.target.innerHTML == "&nbsp;") {
                value++;
            } else {
                value = +event.target.innerHTML;
                value++;
            }

            if (value > 9)
                value = "&nbsp;";

            event.target.innerHTML = value;
            let current = getCurrentPuzzle();
            getPuzzle(current);
        });
    }
}

function modal(message) {
    const modal = document.getElementById('customModal');
    modal.querySelector('p').textContent = message;
    modal.style.display = 'block';
    modal.querySelector('.close').onclick = function () {
        modal.style.display = 'none';
    };
    window.onclick = function (event) {
        if (event.target == modal) {
            modal.style.display = 'none';
        }
    };
}

function modalConfirm(message) {
    return new Promise((resolve, reject) => {
        const modal = document.getElementById('customConfirmModal');
        const cancelBtn = modal.querySelector('#modalCancel');
        const confirmBtn = modal.querySelector('#modalConfirm');

        modal.querySelector('p').textContent = message;
        modal.style.display = 'block';

        modal.querySelector('.close').onclick = function () {
            resolve(false);
            modal.style.display = 'none';
        };

        cancelBtn.onclick = function (event) {
            resolve(false);
            modal.style.display = 'none';
        };

        confirmBtn.onclick = function (event) {
            resolve(true);
            modal.style.display = 'none';
        };

        window.onclick = function (event) {
            if (event.target == modal) {
                resolve(false);
                modal.style.display = 'none';
            }
        };
    });
}

function getCurrentPuzzle() {
    const rootElem = document.getElementById("root");
    let puzzle = "";
    for (let i = 0; i < rootElem.children.length; i++)
        if (rootElem.children[i].innerHTML == "&nbsp;")
            puzzle += "0";
        else
            puzzle += rootElem.children[i].innerHTML;

    return puzzle;
}

function disableAll() {
    document.getElementById("root").style.backgroundColor = "#36ff33";
    document.getElementById("puzzleSelect").disabled = true;
    document.getElementById("addNew").disabled = true;
    document.getElementById("updatePuzzle").disabled = true;
    document.getElementById("deletePuzzle").disabled = true;
    document.getElementById("showPuzzle").disabled = true;
    document.getElementById("resetPuzzle").disabled = true;
}

function enableAll() {
    document.getElementById("root").style.backgroundColor = "#000000";
    document.getElementById("puzzleSelect").disabled = false;
    document.getElementById("addNew").disabled = false;
    document.getElementById("updatePuzzle").disabled = false;
    document.getElementById("deletePuzzle").disabled = false;
    document.getElementById("showPuzzle").disabled = false;
    document.getElementById("resetPuzzle").disabled = false;
}