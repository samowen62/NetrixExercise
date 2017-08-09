$(document).ready(function () {
    $(".datatable").dataTable();

    $(".addStudentButton").on("click", function (e) {
        $("#teacherIDInput").val(e.target.dataset.teacherid);
    });
});