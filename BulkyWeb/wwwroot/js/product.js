function loadDataTable() {
    $('#tblData').DataTable({
        "ajax": {url: '/admin/product/getall'},
        "columns": [
            {data: 'title', "width": "25%"},
            {data: 'isbn', "width": "15%"},
            {data: 'listPrice', "width": "10%"},
            {data: 'author', "width": "15%"},
            {data: 'category.name', "width": "10%"},
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                            <a href="/admin/product/upsert?id=${data}" class="btn btn-primary">
                                <i class="bi bi-pencil-square"></i> Edit
                            </a>
                            <a href="/admin/product/delete?id=${data}" class="btn btn-danger">
                                <i class="bi bi-trash-fill"></i> Delete
                            </a>
                            </div>`
                },
                "width": "25%"
            }
        ]
    });
}

$(document).ready(function () {
    loadDataTable();
});

