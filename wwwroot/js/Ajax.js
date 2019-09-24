function animAra(animName) {
    $.ajax({
        type: "POST",
        url: "",
        data: { aName: animName },
        success: function (myData) {
            $.each(JSON.parse(myData), function (idx, obj) {
                contentMaker(obj);
            });
        },
    });
}
function zamanAralik(ilkZaman, sonZaman) {
    $.ajax({
        type: "POST",
        url: "/ajax/animePaging",
        data: { iZaman: ilkZaman, sZaman: sonZaman },
        success: function (myData) {
            $.each(JSON.parse(myData), function (idx, obj) {
                contentMaker(obj);
            });
        },
    });
}
function zamanEQ(zaman, Equal) {
    $.ajax({
        type: "POST",
        url: "",
        data: { zaman: zaman, EQ: Equal },
        success: function (myData) {
            $.each(JSON.parse(myData), function (idx, obj) {
                contentMaker(obj);
            });
        },
    });
}