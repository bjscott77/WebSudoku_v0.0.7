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
            let data = translateResponseData(rawData);
            hydrateAll(data);   
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
                hydrateRatingElem(data);
            })
            .catch(err => console.log(err));
    }

}

function addPuzzle() {
    let puzzle = document.getElementById("newPuzzleInput");
    let rating = document.getElementById("newPuzzleRating");

    if ((puzzle.value == null && rating.value == null) || (puzzle.value == "" && rating.value == "") || (puzzle.value == 'undefined' && rating.value == 'undefined')) {
        showModal("Please enter a puzzle and rating to save it.");
        return;
    }
    if (rating.value == null || rating.value == "" || rating.value == 'undefined')
        rating.value = 0;

    let newObj = { boardValues: puzzle.value, id: 0, difficulty: +rating.value };

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
            let data = translateResponseData(rawData);
            if (data.StatusCode == "409") {
                showModal(data.Status + ": " + data.ErrorMessage);
            } else if (data.StatusCode == "200") {
                hydrateAll(data);   
            }

        })
        .catch(error => console.error('Error:', error));
}

function updatePuzzle() {
    let uPuzzle = document.getElementById("updatePuzzleInput");
    let sPuzzle = document.getElementById("puzzleSelect");
    let uRating = document.getElementById("updatePuzzleRating");
    let sRating = document.getElementById("selectPuzzleRating");

    if ((uPuzzle.value == null && uRating.value == null) || (uPuzzle.value == "" && uRating.value == "") || (uPuzzle.value == 'undefined' && uRating.value == 'undefined')) {
        showModal("Please select a puzzle to update it.");
        return;
    }

    if (uPuzzle.value == sPuzzle.value && uRating.value == sRating.value) {
        showModal("No changes were specified, so no updates were made.");
        return;
    }

    if (uRating.value == null || uRating.value == "" || uRating.value == 'undefined')
        uRating.value = 0;

    let putObj = [{ boardValues: sPuzzle.value, id: 0, difficulty: 0 }, { boardValues: uPuzzle.value, id: 0, difficulty: +uRating.value }];

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
            hydrateRatingElem(data);

        })
        .catch(error => console.error('Error:', error));
}

function solvePuzzle() {
    disableAll();

    const rootElem = document.getElementById("root");
    let puzzle = "";
    for (let i = 0; i < rootElem.children.length; i++)
        if (rootElem.children[i].innerHTML == "&nbsp;")
            puzzle += "0";
        else
            puzzle += rootElem.children[i].innerHTML;

    console.log("Solve Puzzle", puzzle);

    if (puzzle == null) {
        showModal("Please select a puzzle to solve it.");
    } else {
        fetch("api/sudoku/getsolvedpuzzle?puzzle=" + puzzle)
            .then(res => res.json())
            .then((rawData) => {
                let data = translateResponseData(rawData);
                hydrateRootElem(data);

                enableAll();
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
function toggleNewPuzzleForm() {
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
function toggleUpdatePuzzleForm() {
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

function hydrateAll(puzzles) {
    hydrateSelectElem(puzzles);
    hydrateRatingElem(puzzles);
    hydrateRootElem(puzzles);
}

function hydrateRatingElem(puzzles) {
    let rating = document.getElementById("selectPuzzleRating");
    rating.value = puzzles.Payload[0].difficulty;
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

        });
    }
}

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