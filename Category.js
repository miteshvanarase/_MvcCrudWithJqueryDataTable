
////INSERT CATEGORY INFORMATION
function save() {
    debugger;

    var param = new Object();
    param['CategoryName'] = $("#CategoryName").val();


    $.ajax({
        method: "POST",
        url: "/Test/Submit",
        data: param,
        success: function (data) {
            debugger;
            if (data == 0) {
                alert('Failed to add the Product');
            }
            else {
                alert('Category ' + data.CategoryName + ': Added Successfully')
            }
            $("#CategoryName").val('');
            $("#CategoryName").focus();

        }
    });
}

////UPDATE CATEGORY DETAILS
function update() {
    debugger;

    var param = new Object();
    param['CategoryId'] = $("#CategoryId").val();
    param['CategoryName'] = $("#CategoryName").val();


    $.ajax({
        method: "POST",
        url: "/Test/UpdateCategoryById",
        data: param,
        success: function (data) {
            debugger;
            alert('Category ' + data.CategoryName + ': Updated Successfully')
            window.location.href = "/Test/ListOfCategory";

        }
    });
}

///DELETE CATEGORY DETAILS
function Delete(id) {

    debugger;
    var param = new Object();
    param['CategoryId'] = id;


    $.ajax({
        method: "POST",
        url: "/Test/DeleteCategory",
        data: param,
        success: function (data) {
            debugger;
            alert('Category ' + data.CategoryId + ': Deleted Successfully')
            window.location.href = "/Test/ListOfCategory";
        }
    });
}
///REDIRECT TO EDIT VIEW
function EditCategory(id) {
    debugger;
    window.location.href = "/Test/EditCategory?CategoryId=" + id;
}
////VIEW CATEGORY DETAILS
$(document).ready(function () {

    debugger;
    $('#tblcategory').DataTable({
        "processing": true,
        "serverSide": true,
        "filter": false,
        "info": false,
        "ordermulti": false,
        "ajax": {
            "url": "/Test/CategoryList",
            "type": "POST",
            "datatype": "json"
        },
        "columns": [
             { "data": "CategoryId", "name": "CategoryId", "autowidth": true },
            { "data": "Name", "name": "Name", "autowidth": true },

            {
                'data': null,
                'render': function (data, type, row) {
                    return '<button id="' + row.CategoryId + '" onclick="EditCategory(' + row.CategoryId + ')">Edit</button>'
                }
            },
            {
                'data': null,
                'render': function (data, type, row) {
                    return '<button id="' + row.CategoryId + '"  onclick="Delete(' + row.CategoryId + ')">Delete</button>'
                }
            }

        ]

    })
});
