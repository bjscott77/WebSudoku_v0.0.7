window.onload = function () {
    this.getAllPuzzles();

    let selectElem = document.getElementById("puzzleSelect");
    selectElem.addEventListener("change", function (event) {
        getPuzzle();
    });

    let AddElem = document.getElementById("newPuzzleInput");
    AddElem.addEventListener("focus", function (event) {
        let btn = document.getElementById("addNew");
        btn.innerHTML = "Add";
    });

    let updateElem = document.getElementById("updatePuzzleInput");
    updateElem.addEventListener("focus", function (event) {
        let btn = document.getElementById("updatePuzzle");
        btn.innerHTML = "Update";
    });
};

function getAllPuzzles() {
    fetch("api/sudoku/getallpuzzles")
        .then(res => res.json())
        .then((rawData) => {
            let data = translateResponseData(rawData);
            hydrateSelectElem(data);
            hydrateRootElem(data);
        })
        .catch(err => {
            console.log(err);
        });
}

function getPuzzle() {
    let selectElem = document.getElementById("puzzleSelect");
    let puzzle = selectElem.value.toString();

    if (puzzle == null) {
        showModal("Please select a puzzle to load it.");
    } else {
        fetch("api/sudoku/getpuzzle?puzzle=" + puzzle)
            .then(res => res.json())
            .then((rawData) => {
                let data = translateResponseData(rawData);
                hydrateRootElem(data);
            })
            .catch(err => console.log(err));
    }

}

function addPuzzle() {
    let puzzle = document.getElementById("newPuzzleInput");
    if (puzzle.value == null || puzzle.value == "" || puzzle.value == 'undefined') {
        showModal("Please enter a puzzle to save it.");
        return;
    }
    let jsonPuzzle = JSON.parse("{\"boardValues\":\"" + puzzle.value + "\",\"difficulty\":0, \"id\":0 }");

    fetch("api/sudoku/addpuzzle", {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(jsonPuzzle)
    })
        .then(response => {
            return response.json();
        })
        .then(rawData => {
            let data = translateResponseData(rawData);
            if (data.StatusCode == "409") {
                showModal(data.Status + ": " + data.ErrorMessage);
            } else if (data.StatusCode == "200") {
                hydrateSelectElem(data);
            }

        })
        .catch(error => console.error('Error:', error));
}

function updatePuzzle() {
    let uPuzzle = document.getElementById("updatePuzzleInput");
    let sPuzzle = document.getElementById("puzzleSelect");

    if (uPuzzle.value == null || uPuzzle.value == "" || uPuzzle.value == 'undefined') {
        showModal("Please select a puzzle to update it.");
        return;
    }

    if (uPuzzle.value == sPuzzle.value) {
        showModal("No changes were specified, so no updates were made.");
        return;
    }

    let putObj = [{ boardValues: sPuzzle.value, id: 0, difficulty: 0 }, { boardValues: uPuzzle.value, id: 0, difficulty: 0 }];

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
            data = translateResponseData(rawData);
            hydrateRootElem(data)
            data.Payload = data.Payload.slice(1, data.length);
            hydrateSelectElem(data);

        })
        .catch(error => console.error('Error:', error));
}

function solvePuzzle() {
    let selectElem = document.getElementById("puzzleSelect");
    let puzzle = selectElem?.value?.toString();

    if (puzzle == null) {
        showModal("Please select a puzzle to solve it.");
    } else {
        fetch("api/sudoku/getsolvedpuzzle?puzzle=" + puzzle)
            .then(res => res.json())
            .then((rawData) => {
                let data = translateResponseData(rawData);
                hydrateRootElem(data);
            })
            .catch(err => console.log(err));
    }
}

function deletePuzzle() {
    let puzzle = document.getElementById("puzzleSelect")?.value;

    if (puzzle == null || puzzle == "" || puzzle == 'undefined') {
        showModal("Please select a puzzle to delete it.");
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
                console.log('Success:', data);
            })
            .catch(error => console.error('Error:', error));
    }
}

let even = false;
function toggleNewPuzzleDisplay() {
    let elem = document.getElementById("addToggle");
    let button = document.getElementById("addNew");
    let updatebtn = document.getElementById("updatePuzzle");
    let deletebtn = document.getElementById("deletePuzzle");
    let showbtn = document.getElementById("showPuzzle");
    let resetbtn = document.getElementById("resetPuzzle");

    if (even) {
        let newPuzzle = document.getElementById("newPuzzleInput")?.value;

        if (newPuzzle == null || newPuzzle == "" || newPuzzle == 'undefined') {
            if (button.innerHTML == "Add")
                showModal("No puzzle was entered, so no new puzzles were added.");

            button.innerHTML = "Add..."
            elem.style.display = 'none';
            updatebtn.style.display = 'block';
            deletebtn.style.display = 'block';
            showbtn.style.display = 'block';
            resetbtn.style.display = 'block';
        } else {
            this.addPuzzle();
            button.innerHTML = "Add..."
            elem.style.display = 'none';
            updatebtn.style.display = 'block';
            deletebtn.style.display = 'block';
            showbtn.style.display = 'block';
            resetbtn.style.display = 'block';
        }

    } else {
        button.innerHTML = "Back"
        elem.style.display = 'block';
        updatebtn.style.display = 'none';
        deletebtn.style.display = 'none';
        showbtn.style.display = 'none';
        resetbtn.style.display = 'none';

    }
    even = !even;
}


let odd = true;
function toggleUpdatePuzzleDisplay() {
    let elem = document.getElementById("updateToggle");
    let button = document.getElementById("updatePuzzle");
    let addbtn = document.getElementById("addNew");
    let deletebtn = document.getElementById("deletePuzzle");
    let showbtn = document.getElementById("showPuzzle");
    let resetbtn = document.getElementById("resetPuzzle");

    if (!odd) {
        let updatePuzzle = document.getElementById("updatePuzzleInput")?.value;

        if (updatePuzzle == null || updatePuzzle == "" || updatePuzzle == 'undefined') {
            if (button.innerHTML == "Update")
                showModal("No puzzle was entered, so no puzzles were updated.");

            button.innerHTML = "Update..."
            elem.style.display = 'none';
            addbtn.style.display = 'block';
            deletebtn.style.display = 'block';
            showbtn.style.display = 'block';
            resetbtn.style.display = 'block';
        } else {
            this.updatePuzzle();
            button.innerHTML = "Update..."
            elem.style.display = 'none';
            addbtn.style.display = 'block';
            deletebtn.style.display = 'block';
            showbtn.style.display = 'block';
            resetbtn.style.display = 'block';
        }

    } else {
        let selectPuzzle = document.getElementById("puzzleSelect");
        let updatePuzzle = document.getElementById("updatePuzzleInput");

        updatePuzzle.value = selectPuzzle.value;
        button.innerHTML = "Back"
        elem.style.display = 'block';
        addbtn.style.display = 'none';
        deletebtn.style.display = 'none';
        showbtn.style.display = 'none';
        resetbtn.style.display = 'none';

    }
    odd = !odd;
}

function translateResponseData(puzzles) {
    let data = JSON.parse(puzzles);
    return data;
}

function hydrateSelectElem(puzzles) {
    let select = document.getElementById("puzzleSelect");
    select.innerHTML = "";

    for (let i = 0; i < puzzles.Payload.length; i++) {
        select.innerHTML += "<option>" + puzzles.Payload[i].boardValues + "</option>\r\n";
    }
}

function hydrateRootElem(puzzles) {
    let select = document.getElementById("puzzleSelect").value;
    let root = document.getElementById("root");
    let rootInnerHTML = "";
    let puzzle = puzzles.Payload[0].boardValues;

    for (let i = 0; i < puzzle.length; i++) {
        rootInnerHTML += "<div class='cell'>" + puzzle[i] + "</div>";
    }

    if (puzzles.CellDisplayValueType == "SPACE") {
        rootInnerHTML = rootInnerHTML.replaceAll("0", "&nbsp");
    }

    root.innerHTML = rootInnerHTML;
}

function swapArrayElements(array, index1, index2) {
    [array[index1], array[index2]] = [array[index2], array[index1]];
    return array;
};

function showModal(message) {
    var modal = document.getElementById('customModal');
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