let index = 0;

$(document).ready(function () {
    $("form").on("submit", function () {
        $("#TagList option").prop("selected", "selected");
    })
})



//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
function AddSearchTerm() {
    var searchEntry = document.getElementById("SearchTerm")
    let newOption = new Option(searchEntry.value);
    document.getElementById("SearchTerm").options[0] = newOption;
} 

function AddModBodyAndReason() {
    var modEditText = document.getElementById("ModBodyEdit")
    let newOption = new Option(modEditText.value);
    document.getElementById("ModBodyEdit").options[0] = newOption;

    var modRea = document.getElementById("ModReason")
    let newOption2 = new Option(ModReason.selectedIndex);
    document.getElementById("ModBodyEdit").options[0] = newOption2;
}
//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

function AddTag() {
    // get a reference to the tagentry input element
    var tagEntry = document.getElementById("TagEntry")

    //use search function to look for error state (duplicate or empty tag)
    let tagStr = new Option(tagEntry.value);
    let SearchResult = search(tagEntry.value);
    if (SearchResult != null) 
    
         //trigger search results for error tag (alert)
        Swal.fire({
            // 19, 21:00 no idea how he got the sweetalert css - can't find it in the api
            //html: `<span class = "text-danger text-opacity-50 font-weight-bold"> ${SearchResult.toUpperCase()}</span>`,
            title: 'Error!',
            text: `${SearchResult}`,
            icon: 'error',
            confirmButtonText: 'ok'
        });

    
    else {
        //create a new select option
        let newOption = new Option(tagEntry.value);
        document.getElementById("TagList").options[index++] = newOption;
    }
    //clear out the TagEntry control
    tagEntry.value = "";
    return true;
}

//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
function DeleteTag() {
    // I altered the code below so that there will be no pop up unless somethign is selected prior to hitting delete to save users time
    let tagList = document.getElementById("TagList");
    if (!tagList) return false;
   // var tagEntry = document.getElementById("TagList")
    let tagCount = 1;
    //prompt swal if user hits delete button while nothing is in list
    //let tagList = document.getElementById("TagList");
    

    if (tagList.selectedIndex == -1) {
        Swal.fire({
            title: 'Error!',
            icon: 'error',
            confirmButtonText: 'ok',
            html: "choose a tag before deleting"
        });
        return true;
    }

    while (tagCount > 0) {
        if (tagList.selectedIndex >= 0) {
            tagList.options[tagList.selectedIndex] = null;
            --tagCount;
        }
        else {
            tagCount = 0;
            }
        index--
    }
}

//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

//look too see if tag given has data or is empty
let tv = tagValues;
if (tagValues != '') {
    let tagArray = tagValues.split(',');
    for (let loop = 0; loop < tagArray.length; loop++) {
        // add or replace the options
        ReplaceTag(tagArray[loop], loop);
        index++
    }
}

//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

function ReplaceTag(tag, index) {
    let newOption = new Option(tag, tag)
    document.getElementById("TagList").options[index] = newOption;
}
//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

// the search function will detect either an empty or a duplicate Tag
// and return an error string if an error // detect//
function search(str) {
    if (str == "") {
        return "Emtpy tags are not permitted";
    }
    var tagsEl = document.getElementById('TagList');
    if (tagsEl.options.length > 0) {
        let options = tagsEl.options;
        for (let i = 0; i < options.length; i++) {
            if (options[i].value == str) {
                return `The Tag #${str} is a duplicate. Duplicate tags are not permitted`
            }
        }
    }
}
//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

const swalwithDarkButton = Swal.mixin({
    customclass: {
        confirmButton: 'btn btn-danger btn-sm btn-block btn-outline-dark'
    },
    imageUrl: '/assets/img/incorrectImage.jpg',
    timer: 3000,
    buttonStyling: false
});
//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

function editComment() {
    var tagEntry = document.getElementById("TagEntry")

    swalwithDarkButton.fire({
        html: "text",
        title: 'Error!',
        text: "gg",
        icon: "error",
        confirmButtonText: 'ok'
    })
};

//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

//swal.fire({
//            title: 'Error!',
//            text: `${SearchResult}`,
//            icon: 'error',
//            confirmButtonText: 'ok'
//        })




















//////$(document).ready(function () {
//    $("form").on("submit", function () {
//        $("#TagList").prop("selected", "selected");
//    })
////})

//////$("form").on("submit", function (e) {
//////    e.preventDefault();
//////    $("#TagList option").prop("selected", "selected");
//////    (e).stopPropagation();
//////})

//////swal.fire({
////        //    title: 'Error!',
////        //    text: `${SearchResult}`,
////        //    icon: 'error',
////        //    confirmButtonText: 'ok'
////        //})