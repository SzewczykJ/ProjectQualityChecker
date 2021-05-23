$(document).ready(function () {
    let stored_JSON;
    let metrics;
    loadJSON(function (response) {
        metrics = JSON.parse(response);
    });

    function BuildChart(dataset, selectedMetric, graph, xAxisGraphContext) {
        Chart.defaults.font.size = 14;
        Chart.defaults.font.family = "'Montserrat', sans-serif";
        let rectangleSet = false;

        metrics[selectedMetric].graphCanvas = new Chart(graph,
            {
                type: 'line',
                data: {
                    labels: dataset.map(el => el.Sha.substring(0, 6)),
                    datasets: [{
                        data: dataset.filter(commit => commit.Metrics != null)
                            .map(commit => commit.Metrics[selectedMetric]),
                        lineTension: 0.2,
                        backgroundColor: "rgb(128,126,126)",
                        pointBackgroundColor: new Array(dataset.length).fill("#fff"),
                        pointBorderColor: new Array(dataset.length).fill("rgb(134,133,133)"),

                        borderWidth: 2,
                        borderColor: "rgba(220,220,220,1)",
                        pointBorderWidth: 1,
                        pointRadius: 10,
                        pointHoverRadius: 10,
                        pointHoverBackgroundColor: "#6f42c1",
                        pointHoverBorderColor: "#6610f2",
                        pointHoverBorderWidth: 3
                    }],
                },
                options: {
                    title: {
                        display: false
                    },
                    legend:
                        {
                            display: false
                        },
                    responsive: true, // Instruct chart JS to respond nicely.
                    maintainAspectRatio: false, // Add to prevent default behavior of full-width/height 
                    tooltips: {
                        mode: 'index',
                        backgroundColor: 'rgba(255,255,255)',
                        borderColor: 'rgb(0, 0, 0)',
                        borderWidth: 0.3,
                        cornerRadius: 1,
                        caretSize: 0,
                        xPadding: 7,
                        yPadding: 5,
                        titleFontColor: 'rgba(0, 0, 0, 0.87)',
                        titleFontSize: 10,
                        titleFontFamily: 'Roboto',
                        bodyFontFamily: 'Roboto',
                        callbacks: {
                            title: function (toolTipItem, data) {
                                return metrics[selectedMetric].displayName;
                            },
                            label: function (toolTipItem, data) {
                                return toolTipItem.dataset.data[toolTipItem.dataIndex].toFixed(2);
                            },
                            afterLabel: function (toolTipItem, data) {
                                let text = [''];
                                text.push("Date:   " + new Date(dataset[toolTipItem.dataIndex].Date).toDateString("yyyy-MM-dd"));
                                text.push("Author:   " + dataset[toolTipItem.dataIndex].Developer.Username);
                                text.push("Sha:   " + dataset[toolTipItem.dataIndex].Sha);
                                text.push("Message:   " + dataset[toolTipItem.dataIndex].Message.substr(0, 100) + "...");
                                return text;
                            },
                            labelTextColor: function (tooltipItem, chart) {
                                return '#0a0d1c';
                            }
                        },
                    },
                    scales: {
                        xAxes: [{
                            ticks: {
                                autoSkip: false,
                                maxRotation: 90,
                                minRotation: 90,
                                fontSize: 12
                            },
                            gridLines: {
                                display: false
                            },
                        }],
                        yAxes: [{
                            ticks: {
                                fontSize: 12,
                                beginAtZero: true
                            }
                        }]
                    },
                    layout: {
                        padding: 10
                    },
                    onClick: function (evt, activeElements) {
                        const elementIndex = activeElements[0].index;
                        Object.keys(metrics).forEach(function (value, index) {
                            metrics[value].graphCanvas.data.datasets[0].pointBackgroundColor.fill("#fff");
                            metrics[value].graphCanvas.data.datasets[0].pointBorderColor.fill("rgba(220,220,220,1)");
                            metrics[value].graphCanvas.data.datasets[0].pointBackgroundColor[elementIndex] = "#6f42c1";
                            metrics[value].graphCanvas.data.datasets[0].pointBorderColor[elementIndex] = "#6610f2";
                            metrics[value].graphCanvas.update();
                        });

                    },
                    animation: {
                        onComplete: function () {
                            if (!rectangleSet) {
                                const scale = window.devicePixelRatio;
                                const sourceCanvas = metrics[selectedMetric].graphCanvas.canvas;
                                let copyWidth = metrics[selectedMetric].graphCanvas.scales['yAxes'].width;
                                let copyHeight = metrics[selectedMetric].graphCanvas.scales['yAxes'].height + metrics[selectedMetric].graphCanvas.scales['yAxes'].top + 10;
                                const targetCtx = xAxisGraphContext;

                                targetCtx.canvas.style.width = `${copyWidth}px`;
                                targetCtx.canvas.style.height = `${copyHeight}px`;

                                copyWidth = copyWidth * scale;
                                copyHeight = copyHeight * scale;
                                targetCtx.canvas.width = copyWidth;
                                targetCtx.canvas.height = copyHeight;

                                targetCtx.drawImage(sourceCanvas, 0, 0, copyWidth, copyHeight, 0, 0, copyWidth, copyHeight);
                                const sourceCtx = sourceCanvas.getContext('2d');
                                sourceCtx.clearRect(0, 0, copyWidth, copyHeight);
                                rectangleSet = true;
                                metrics[selectedMetric].graphCanvas.update();
                            }
                        },
                        onProgress: function () {
                            if (rectangleSet === true) {
                                const copyWidth = metrics[selectedMetric].graphCanvas.scales['yAxes'].width;
                                const copyHeight = metrics[selectedMetric].graphCanvas.scales['yAxes'].height + metrics[selectedMetric].graphCanvas.scales['yAxes'].top + 10;

                                const sourceCtx = metrics[selectedMetric].graphCanvas.canvas.getContext('2d');
                                sourceCtx.clearRect(0, 0, copyWidth, copyHeight);
                            }
                        }
                    }
                },
                plugins: [{
                    beforeRender: function (c, options) {
                        var yScale = c.scales['yAxes'].max;
                        var metricData = metrics[selectedMetric];
                        var totalSteps = metricData.valueLevel.length;
                        var gradientFill = c.ctx.createLinearGradient(0, c.height, 0, 0);

                        $.each(Object.keys(metricData.valueLevel), function (index) {
                            if (index === 0 || index === (totalSteps - 1)) {
                                gradientFill.addColorStop(metricData.valueLevel[index], metricData.colors[index]);
                            } else {
                                console.log(calculateGradientStep(yScale, metricData.valueLevel[index]));
                                gradientFill.addColorStop(calculateGradientStep(yScale, metricData.valueLevel[index]), metricData.colors[index]);
                            }
                        });

                        c.data.datasets[0].backgroundColor = gradientFill;

                    }
                }]
            });
    }


    let AddCanvasToPage = function (metric) {
        $('#graphs').append(
            $('<div/>', {
                class: 'row justify-content-center my-5'
            }).append(
                $('<div/>', {
                    class: 'col-12 mb-4'
                }).append(
                    $('<p/>').append(
                        $('<button/>',
                            {
                                class: "btn btn-primary",
                                text: metrics[metric].displayName,
                                type: "button",
                                "data-toggle": "collapse",
                                "data-target": '#' + metric + "Description",
                                "aria-expanded": false
                            }).append($('<span/>', {
                            class: "fas fa-angle-down js-rotate-if-collapsed ",
                            "aria-hidden": true
                        }))
                    ),
                    $('<div/>', {
                        class: "collapse mt-2",
                        "data-toggle": "false",
                        id: metric + "Description"
                    }).append(
                        $('<div/>', {
                            class: "card card-body",
                            html: metrics[metric].description
                        })
                    )
                ),
                $('<div/>', {
                    class: 'chartWrapper col-12 p-0'
                }).append(
                    $('<div/>', {
                        class: 'chartAreaWrapper'
                    }).append(
                        $('<div/>', {
                            class: 'chartAreaWrapper2'
                        }).append(
                            $('<canvas/>',
                                {
                                    id: metric,
                                    width: 1200,
                                    height: 400
                                })
                        ),
                    ),
                    $('<canvas/>',
                        {
                            id: "xAxis" + metric,
                            width: 0,
                            height: 400
                        })
                ))
        );


    };

    function GenerateGraphs() {
        $.each(Object.keys(metrics), function (index, metric) {
            // remove old charts          
            if ($('#' + metric).length !== 0) {
                $('#' + metric).parent().parent().parent().parent().remove();
            }

            const data = stored_JSON.CommitList
                .filter(commit => commit.Metrics != null)
                .map(commit => commit);
            if (data.length === 0) return;

            AddCanvasToPage(metric);

            let selectedGraph = $('#' + metric)[0];
            let context = selectedGraph.getContext('2d');

            let selectedXAxisGraph = $("#xAxis" + metric)[0];
            let xAxiscontext = selectedXAxisGraph.getContext('2d');

            let newWidth = (data.length * 30) + 60;
            $('.chartAreaWrapper2').width(newWidth);

            BuildChart(data, metric, context, xAxiscontext);
        });
    }


    //Switch send button style
    let selectedRepository = $('#repository');
    var $submitbutton = $("#submitbutton");

    selectedRepository.on('change', function () {

        if (selectedRepository.val() != false) {
            $submitbutton.removeAttr("disabled");
            $submitbutton.removeClass("btn-secondary");
            $submitbutton.addClass("btn-primary");
        } else {
            $submitbutton.attr("disabled", "disabled");
            $submitbutton.addClass("btn-secondary");
            $submitbutton.removeClass("btn-primary");
        }
    });


    // Ajax query 
    $('button[name="repository_confirm"]').click(function () {
        let selectedRepository = $('#repository').val();
        let selectedBranch = $('#branches').val();

        if (selectedRepository == false || selectedBranch == false) return;
        $.ajax({
            method: "POST",
            url: "/result/getresult",
            data: {
                RepositoryId: selectedRepository,
                BranchId: selectedBranch
            },
            success: function (response) {
                $('#chartImage').hide();
                $('#chartImageError').hide();

                stored_JSON = response["CommitSummary"];
                GenerateGraphs();
                $('.collapse').collapse('hide');

                if ($('.chartAreaWrapper').length === 0) {
                    $('#chartImageError').show();
                }

                $('.chartAreaWrapper').scrollsync();
            },
            error: function (response) {
                console.log(response);
            }
        });
    });

    function calculateGradientStep(oldMax, value) {
        return (1 / oldMax) * (value - oldMax) + 1;
    }

    function loadJSON(callback) {

        var xobj = new XMLHttpRequest();
        xobj.overrideMimeType("application/json");
        xobj.open('GET', 'js/MetricsInfo.json', true);
        xobj.onreadystatechange = function () {
            if (xobj.readyState == 4 && xobj.status == "200") {
                callback(xobj.responseText);
            }
        };
        xobj.send(null);
    }

});
  