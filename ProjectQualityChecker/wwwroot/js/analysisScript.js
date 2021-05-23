$(document).ready(function () {

    function GetListOfBranches(repositoryURL) {
        var url = new URL(repositoryURL);
        var path = url.pathname.split('/');
        SuggestProjectName(path[2]);

        if (url.hostname.match("github.com")) {
            CallGitHubAPI(path[1], path[2]);
        } else if (url.hostname.match("bitbucket.org")) {
            CallBitbucketAPI(path[1], path[2]);
        }

    }

    function SuggestProjectName(repositoryName) {
        var projectNameField = $('#name');
        if (!projectNameField.val()) {
            projectNameField.val(repositoryName);
        }
    }

    function CallBitbucketAPI(repository, workspace) {
        var apiLink = "https://api.bitbucket.org/2.0/repositories/" + repository + "/" + workspace + "/refs/branches";
        $.ajax({
            method: "GET",
            url: apiLink,
            dataType: "json",
            success: function (response) {
                var data = response.values.map(branch => branch.name);
                if (data !== null) {
                    var branchSelect = $('#branch');
                    branchSelect.removeAttr("disabled");
                    data.forEach(function (branch) {
                        branchSelect.append($('<option>', {
                            value: branch,
                            text: branch
                        }));
                    });
                }
            },
            error: function (response) {
                Swal.fire(
                    "Can not download this repository",
                    "Check is it set to the public repository and try again.",
                    'error'
                );
            }
        });
    }

    function CallGitHubAPI(user, repository) {
        var apiLink = "https://api.github.com/repos/" + user + "/" + repository + "/branches";
        $.ajax({
            method: "GET",
            url: apiLink,
            dataType: "json",
            success: function (response) {
                var data = response.map(branch => branch.name);
                if (data !== null) {
                    var branchSelect = $('#branch');
                    branchSelect.removeAttr("disabled");
                    data.forEach(function (branch) {
                        branchSelect.append($('<option>', {
                            value: branch,
                            text: branch
                        }));
                    });
                }
            },
            error: function (response) {
                Swal.fire(
                    "Can not download this repository",
                    "Check is it set to the public repository and try again.",
                    'error'
                );
            }
        });
    }

    function ClearBranchSelect() {
        var branchSelect = $('#branch');
        branchSelect.prop("disabled", true);
        branchSelect.find('option').remove().end();
    }


    $('#link').change(function () {
        var linkField = $('#link');
        linkField.removeClass('error').next($('#link-error')).remove();
        var validator = linkField.validate({
            rules: {
                link: {
                    required: true,
                    url: true,
                    minLength: 5
                },
            },
            messages: {
                link: "Please enter link to repository",
            },
            errorClass: "text-danger",
            success: function (label, element) {
                ClearBranchSelect();
                GetListOfBranches(element.value);
            }
        });

        validator.element("#link");
    });


    $('#form').validate({
        rules: {
            name: {required: true},
            link: {
                required: true,
                url: true,
                minLength: 5
            },
            branch: {required: true}
        },
        messages: {
            name: "Please enter project name",
            link: "Please enter link to repository",
        },
        errorClass: "text-danger",

        submitHandler: function (form) {
            let name = $('#name').val();
            let url = $('#link').val();
            let branch = $('#branch').val();
            $.ajax({
                method: "POST",
                url: "/analysis/analysis",
                data: {
                    name: name,
                    url: url,
                    branch: branch,
                    '@tokenSet.HeaderName': '@tokenSet.RequestToken'
                },
                datatype: JSON,
                beforeSend: function () {
                    Swal.fire({
                        imageUrl: "/images/loading_circle.svg",
                        showConfirmButton: false,
                        text: "The analysis is in progress and may take several minutes."
                    });
                },
                success: function (response) {
                    Swal.fire(
                        "Repository added",
                        "The analysis is in progress and may take several minutes.",
                        'success'
                    );
                },
                error: function (response) {
                    Swal.fire(
                        "Can not analyze repository",
                        "Please try again if an error has still occurred, please contact support.",
                        'error'
                    );
                }
            });
            return false;
        }
    });
});