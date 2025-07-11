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
            if (data.StatusCode == 200) {
                hydrateAll(data);
                if (data.PuzzleMode == "PLAY")
                    document.getElementById("puzzleToolsSection").style.display = 'block';
                document.getElementById("puzzleMode").innerHTML = "Mode: "+data.PuzzleMode;
            } else {
                modal(data.Status + ": " + data.ErrorMessage);
            }
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
        document.getElementById("resetPuzzle").disabled = true;
        document.getElementById("undo").disabled = true;
        moveRecord = [];
        let currentIndex = 0;
        document.getElementById("markedCellsList").innerHTML = "";
        document.getElementById("selectedCell").children[0].innerHTML = "";
        document.getElementById("markedUndo").disabled = true;
    }

    if (puzzle == null) {
        modal("Please select a puzzle to load it.");
    } else {
        fetch("api/sudoku/getpuzzle?puzzle=" + puzzle + "&id=" + id)
            .then(res => res.json())
            .then((rawData) => {
                const data = translateResponseData(rawData);
                if (data.StatusCode == 200) {
                    if (data.PuzzleMode == "SOLVE") {
                        hydrateRootElem(data);
                    } else {
                        hydrateRootElemPlay(data);
                    }
                    hydrateRatingElem(data);
                } else if (data.Solved) {
                    modal("Congratulations, you win!");
                    return;
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
    if (puzzles.PuzzleMode == "SOLVE") {
        document.getElementById("undo").style.display = 'none';
        document.getElementById("markedUndo").style.display = 'none';
        hydrateRootElem(puzzles);
    } else {
        document.getElementById("addNew").style.display = 'none';
        document.getElementById("updatePuzzle").style.display = 'none';
        document.getElementById("deletePuzzle").style.display = 'none';
        document.getElementById("solvePuzzle").style.display = 'none';
        hydrateRootElemPlay(puzzles);
    }
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

    /*
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
    */
}

let moveRecord = [];
let currentIndex = 0;
function hydrateRootElemPlay(puzzles) {
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
        root.children[i].index = i;
        root.children[i].addEventListener("click", function (event) {
            currentIndex = event.target.index;
            document.getElementById("selectedCell").children[0].innerHTML = currentIndex;
            modalCellValueInput();
        });
    }
}

function markCell() {
    const index = document.getElementById("selectedCell").children[0].innerHTML;
    const cells = document.getElementById("root").children;
    const possibles = cells[index].getAttribute('data-title');

    if (index == "" || possibles == "") {
        modal("Must have a cell index and possibles selected to mark the info.");
        return;
    }

    let list = document.getElementById("markedCellsList");

    for (let i = 0; i < list.children.length; i++) {
        if (list.children[i].innerHTML.includes(`Cell: ${index} Possibles: ${possibles}`)) {
            modal("This cell is already in the marked list.");
            return;
        }
    }

    const node = document.createElement("li");
    const text = document.createTextNode(`Cell: ${index} Possibles: ${possibles}`);
    node.appendChild(text);
    list.appendChild(node);
    
    document.getElementById("markedUndo").disabled = false;
}

function unMarkLastCell(bypass = false) {
    let list = document.getElementById("markedCellsList");
    if (list.children.length == 0)
        return;

    const lastMarked = list.lastElementChild;
    const index = lastMarked.innerHTML.split(" ")[1];
    if (currentIndex != index && !bypass)
        return;

    list.removeChild(list.lastElementChild);
    document.getElementById("selectedCell").children[0].innerHTML = "";
    if (list.children.length == 0) {
        document.getElementById("markedUndo").disabled = true;
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

function modalCellValueInput() {
    return new Promise((resolve, reject) => {
        const modal = document.getElementById('customInputModal');
        const cancelBtn = modal.querySelector('#inputCancel');
        const confirmBtn = modal.querySelector('#inputConfirm');
        modal.style.display = 'block';
        let input = document.getElementById("value");
        input.value = "";
        input.focus();
        input.oninput = function (event) {
            confirmBtn.focus(); 
        }

        modal.querySelector('.close').onclick = function () {
            resolve(false);
            modal.style.display = 'none';
        };

        cancelBtn.onclick = function (event) {
            resolve(false);
            modal.style.display = 'none';
        };

        confirmBtn.onclick = function (event) {
            if (input.value < 0 || input.value > 9 || input.value == "") {
                resolve(false);
                modal.style.display = 'none';
            } else if (input.value >= 0 && input.value <= 9) {
                moveRecord.push(currentIndex);
                console.log(moveRecord.join(", "));
                if (document.getElementById("inputMark").checked) {
                    markCell();
                    document.getElementById("inputMark").checked = false;
                } else {
                    unMarkLastCell();
                }

                let cell = document.getElementById("root").children[currentIndex];
                cell.innerHTML = input.value;

                if (document.getElementById("undo").disabled)
                    document.getElementById("undo").disabled = false;

                if (document.getElementById("resetPuzzle").disabled)
                    document.getElementById("resetPuzzle").disabled = false;

                let current = getCurrentPuzzle();
                getPuzzle(current);
                resolve(true);
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

function undoLastMove() {
    let cells = document.getElementById("root").children;
    let index = -1;
    if (moveRecord.length > 0)
        index = moveRecord.pop();
    else
        return;

    cells[index].innerHTML = "&nbsp;";

    if (moveRecord.length == 0) {
        document.getElementById("undo").disabled = true;
        document.getElementById("markedUndo").disabled = true;
        document.getElementById("resetPuzzle").disabled = true;
        document.getElementById("markedCellsList").innerHTML = "";
    }

    let current = getCurrentPuzzle();
    getPuzzle(current);
}

function revertToLastMarked() {
    if (moveRecord.length == 0)
        return;

    const elemList = document.getElementById("markedCellsList");
    const cellIndex = elemList.lastElementChild.innerHTML.split(" ")[1];
    const list = Array.from(elemList.children);
    const start = moveRecord.length - 1;
    let record = moveRecord[start];
    for (let i = start; i >= 0; i--) {
        if (record != cellIndex) {
            undoLastMove();
            console.log(`undo: ${moveRecord.join(", ")}`);
            record = moveRecord[moveRecord.length - 1];
        }
        else {
            undoLastMove();
            console.log(`undo: ${moveRecord.join(", ")}`);
            break;
        }
    }

    unMarkLastCell(true);
}
