//this one is temp function for try
function createNode(colour, index) {
    $(".plakaCont").append("<div class='plaka' style='background:" + colour + "'><h1>" + index + "</h1></div>");
}
//this one is creating nodes how many clients want to add
function createPlaka(index) {
    let colour = '#' + Math.random().toString(16).slice(2, 8).toUpperCase()
    for (let i = 0; i < index; i++) {
        createNode(colour, (i + 1));
    }
}
function contentMaker(data) {
    var htmlCode = '<div class="animeContent"><div class="imgCont"><img src=' + data.posterLink + '></div ><div class="infoCont"><div class="baslikCont"><p id="label">Anime Adı : </p><p id="baslik">' + data.name + '</p></div><div class="mangaCont"><p id="label">Kategori : </p><p id="mangaka">' + data.category + '</p></div><div class="tarihCont"><p id="label">Tarih : ' + data.time + '</p></div><a class="myBtn" id="detailBtn" href=' + data.link + '><span id="btnText">details</span></a></div ></div >';
    $(".animeContainer").append(htmlCode)
}
function clearContainer() {
    $(".animeContent").each(function (index) {
        $(this).remove();
    })
}