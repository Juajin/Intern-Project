var tmpResult = 0;
var totalItem = 5;
$(document).ready(function () {
    $(".mainContainer").scroll(function () {

        var sT = parseFloat($(".mainContainer").scrollTop())
        var iH = parseFloat($('.mainContainer').innerHeight());
        var wH = $('.mainContainer')[0].scrollHeight;
        var finalResult = sT + iH + 1.0;
        if (finalResult > wH) {
            //createPlaka(5);
            //Ajax
            totalItem = totalItem + 5;
            $("span#count").empty()
            $("span#count").append(totalItem);
        }
    });
    $(".aBtn").click(function (e) {
        e.preventDefault();
        //Ajax
    })
    $(".zAra").click(function (e) {
        e.preventDefault();
        //Ajax
    })
    $(".zQara").click(function (e) {
        e.preventDefault();
        //Ajax
    })
    $("#zmnGosterBtn").click(function () {
        $(this).css("display", "none");
        $("#katGosterBtn").css("display", "block");
        $(".zmnKategori").css("display", "none");
        $(".kategori").css("display", "none");
        $(".aBtn").css("display", "none");


        $(".zmnAralik").css("display", "inline-block");
        $(".zmnEqual").css("display", "inline-block");
        $(".zmnEqual").css("display", "inline-block");
        $(".zAra").css("display", "inline-block");

        $(".zQara").css("display", "inline-block");


    })
    $("#katGosterBtn").click(function () {
        $(this).css("display", "none");
        $("#zmnGosterBtn").css("display", "block");
        $(".zmnKategori").css("display", "inline-block");
        $(".kategori").css("display", "inline-block");
        $(".aBtn").css("display", "inline-block");

        $(".zmnAralik").css("display", "none");
        $(".zmnEqual").css("display", "none");
        $(".zAra").css("display", "none");

        $(".zQara").css("display", "none");

    })
});
