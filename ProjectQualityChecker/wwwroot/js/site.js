$(document).ready(function () {

    function BuildChart(dataset, selectedProperty, graph, xAxisGraphContext, chartName) {
        Chart.defaults.font.size = 14;
        Chart.defaults.font.family = "'Montserrat', sans-serif";
        let rectangleSet = false;
        const chart =
            {
                type: 'line',
                data: {
                    labels: dataset.map(el => el.Sha.substring(0, 6)),
                    datasets: [{
                        data: dataset.filter(commit => commit.Metrics != null)
                            .map(commit => commit.Metrics[selectedProperty]),
                        lineTension: 0.2,
                        pointBackgroundColor: new Array(dataset.length).fill("#fff"),
                        pointBorderColor: new Array(dataset.length).fill("rgb(134,133,133)"),
                        backgroundColor: "rgba(220,220,220,0.2)",
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
                                return selectedProperty;
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
                                color: new Array(dataset.length).fill("#8c8a8a"),
                                display: false
                            },
                            barThickness: 30,
                            maxBarThickness: 30,
                            minBarLength: 30,
                        }],
                        yAxes: [{
                            ticks: {
                                fontSize: 12,
                                beginAtZero: true
                            }
                        }]
                    },
                    onClick: function (evt, activeElements) {
                        const elementIndex = activeElements[0].index;
                        metricsName.forEach(function (value, index) {
                            chartList[value].data.datasets[0].pointBackgroundColor.fill("#fff");
                            chartList[value].data.datasets[0].pointBorderColor.fill("rgba(220,220,220,1)");
                            chartList[value].data.datasets[0].pointBackgroundColor[elementIndex] = "#6f42c1";
                            chartList[value].data.datasets[0].pointBorderColor[elementIndex] = "#6610f2";
                            chartList[value].update();
                        });

                    },
                    animation: {
                        onComplete: function () {
                            if (!rectangleSet) {
                                const scale = window.devicePixelRatio;
                                const sourceCanvas = chartList[chartName].canvas;
                                let copyWidth = chartList[chartName].scales['yAxes'].width - 5;
                                let copyHeight = chartList[chartName].scales['yAxes'].height + chartList[chartName].scales['yAxes'].top + 10;
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
                            }
                        },
                        onProgress: function () {
                            if (rectangleSet === true) {
                                const copyWidth = chartList[chartName].scales['yAxes'].width;
                                const copyHeight = chartList[chartName].scales['yAxes'].height + chartList[chartName].scales['yAxes'].top + 10;

                                const sourceCtx = chartList[chartName].canvas.getContext('2d');
                                sourceCtx.clearRect(0, 0, copyWidth, copyHeight);
                            }
                        }
                    }
                }
            };
        return new Chart(graph, chart);
    }

    let stored_JSON;
    let metricsName = [
        "Complexity",
        "CognitiveComplexity",
        "DuplicatedLines",
        "CodeSmells",
        // "NewCodeSmells",
        "CommentLines",
        "CommentLinesDensity",
        "Ncloc",
        "Statements",
        // "BranchCoverage",
        // "LineCoverage"
    ];
    let chartList = [];

    let AddCanvasToPage = function (value) {
        $('#graphs').append(
            $('<div/>', {
                class: 'row justify-content-center my-5'
            }).append(
                $('<div/>', {
                    class: 'col-12'
                }).append(
                    $('<span/>', {
                        class: 'badge badge-light p-2 mb-2',
                        text: value
                    })
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
                                    id: value,
                                    width: 1200,
                                    height: 400
                                })
                        ),
                    ),
                    $('<canvas/>',
                        {
                            id: "xAxis" + value,
                            width: 0,
                            height: 400
                        })
                ))
        );

    };

    function GenerateGraphs() {
        $.each(metricsName, function (index, value) {
            // remove old charts          
            if ($('#' + value).length !== 0) {
                $('#' + value).parent().parent().parent().parent().remove();
            }

            const data = stored_JSON.CommitList
                .filter(commit => commit.Metrics != null)
                .map(commit => commit);
            if (data.length === 0) return;

            AddCanvasToPage(value);

            let selectedGraph = $('#' + value)[0];
            let context = selectedGraph.getContext('2d');

            let selectedXAxisGraph = $("#xAxis" + value)[0];
            let xAxiscontext = selectedXAxisGraph.getContext('2d');

            let newWidth = (data.length * 30) + 60;
            $('.chartAreaWrapper2').width(newWidth);
            chartList[value] = BuildChart(data, value, context, xAxiscontext, value);
        });
    }

    $('input[name="repository_confirm"]').click(function () {
        let selectedRepository = $('#repository').val();

        $.ajax({
            method: "GET",
            url: "/result/getresult",
            data: {
                repositoryId: selectedRepository
            },
            success: function (response) {
                $('#chartImage').hide();
                $('#chartImageError').hide();

                stored_JSON = response;
                GenerateGraphs();

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


});
  