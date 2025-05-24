type = ['primary', 'info', 'success', 'warning', 'danger'];
bgColors = ["rgba(255, 99, 132, 0.2)",
	"rgba(255, 159, 64, 0.2)",
	"rgba(255, 205, 86, 0.2)",
	"rgba(75, 192, 192, 0.2)",
	"rgba(54, 162, 235, 0.2)",
	"rgba(153, 102, 255, 0.2)",
	"rgba(201, 203, 207, 0.2)",
	"rgba(255, 99, 132, 0.2)",
	"rgba(255, 205, 86, 0.2)",
	"rgba(75, 192, 192, 0.2)",
	"rgba(255, 159, 64, 0.2)",
	"rgba(54, 162, 235, 0.2)",
	"rgba(153, 102, 255, 0.2)",
	"rgba(201, 203, 207, 0.2)",
	"rgba(255, 99, 132, 0.2)",
	"rgba(255, 159, 64, 0.2)",
	"rgba(255, 205, 86, 0.2)",
	"rgba(201, 203, 207, 0.2)",
	"rgba(75, 192, 192, 0.2)",
	"rgba(54, 162, 235, 0.2)",
	"rgba(255, 99, 132, 0.2)",
	"rgba(255, 159, 64, 0.2)",
	"rgba(153, 102, 255, 0.2)",
	"rgba(54, 162, 235, 0.2)",
	"rgba(255, 205, 86, 0.2)",
	"rgba(75, 192, 192, 0.2)",
	"rgba(153, 102, 255, 0.2)",
	"rgba(201, 203, 207, 0.2)"];
murshadik = {
	// CHARTS
	initDashboardPageCharts: function () {

		chartColor = "#FFFFFF";

		var cardStatsMiniLineColor = "#fff",
			cardStatsMiniDotColor = "#fff";

		Chart.pluginService.register({
			beforeDraw: function (chart) {
				if (chart.config.options.elements.center) {
					//Get ctx from string
					var ctx = chart.chart.ctx;

					//Get options from the center object in options
					var centerConfig = chart.config.options.elements.center;
					var fontStyle = centerConfig.fontStyle || 'Arial';
					var txt = centerConfig.text;
					var color = centerConfig.color || '#000';
					var sidePadding = centerConfig.sidePadding || 20;
					var sidePaddingCalculated = (sidePadding / 100) * (chart.innerRadius * 2)
					//Start with a base font of 30px
					ctx.font = "30px " + fontStyle;

					//Get the width of the string and also the width of the element minus 10 to give it 5px side padding
					var stringWidth = ctx.measureText(txt).width;
					var elementWidth = (chart.innerRadius * 2) - sidePaddingCalculated;

					// Find out how much the font can grow in width.
					var widthRatio = elementWidth / stringWidth;
					var newFontSize = Math.floor(30 * widthRatio);
					var elementHeight = (chart.innerRadius * 2);

					// Pick a new font size so it will not be larger than the height of label.
					var fontSizeToUse = Math.min(newFontSize, elementHeight);

					//Set font settings to draw it correctly.
					ctx.textAlign = 'center';
					ctx.textBaseline = 'middle';
					var centerX = ((chart.chartArea.left + chart.chartArea.right) / 2);
					var centerY = ((chart.chartArea.top + chart.chartArea.bottom) / 2);
					ctx.font = fontSizeToUse + "px " + fontStyle;
					ctx.fillStyle = color;

					//Draw text in center
					ctx.fillText(txt, centerX, centerY);
				}
			}
		});

	},

	lineChart: function (elementId, lineColor, labels, data, label, stepped = false, fill = false, dXAxis = true, dYAxis = true) {
		ctx = document.getElementById(elementId).getContext("2d");

		gradientStroke = ctx.createLinearGradient(500, 0, 100, 0);
		gradientStroke.addColorStop(0, '#80b6f4');
		gradientStroke.addColorStop(1, chartColor);

		gradientFill = ctx.createLinearGradient(0, 170, 0, 50);
		gradientFill.addColorStop(0, "rgba(128, 182, 244, 0)");
		gradientFill.addColorStop(1, "rgba(249, 99, 59, 0.40)");

		myChart = new Chart(ctx, {
			type: 'line',
			data: {
				labels: labels,
				datasets: [{
					label: label,
					borderColor: lineColor,
					pointRadius: 5,
					pointHoverRadius: 5,
					fill: fill,
					borderWidth: 3,
					data: data,
					steppedLine: stepped
				}]
			},
			options: {
				legend: {
					display: false
				},

				tooltips: {
					enabled: true
				},

				scales: {
					yAxes: [{
						ticks: {
							fontColor: "#9f9f9f",
							beginAtZero: false,
							maxTicksLimit: 5,
						},
						gridLines: {
							drawBorder: true,
							display: dYAxis
						}

					}],

					xAxes: [{
						barPercentage: 1.6,
						gridLines: {
							drawBorder: true,
							display: dXAxis,
						},
						ticks: {
							padding: 20,
							fontColor: "#9f9f9f"
						}
					}]
				},
			}
		});
	},

	barChart: function (elementId, labels, data, label, stepSize = 0) {

		ctx = document.getElementById(elementId).getContext("2d");
		myChart = new Chart(ctx,
			{
				type: "bar",
				data: {
					labels: labels,
					datasets: [
						{
							label: label,
							data: data,
							fill: false,
							backgroundColor: this.shuffle(bgColors),
							borderColor: this.shuffle(bgColors),
							borderWidth: 1
						}
					]
				},
				options: {
					legend: {
						display: false
					},
					tooltips: {
						enabled: true
					},
					scales: {
						yAxes: [{
							ticks: {
								beginAtZero: true,
								stepSize: stepSize
							}
						}]
					}
				}
			}
		)
	},

	pieChart: function (elementId, lineColor, bgColors, data, textLabel, label = "") {
		ctx = document.getElementById(elementId).getContext("2d");

		myChart = new Chart(ctx, {
			type: 'pie',
			data: {
				labels: [1, 2],
				datasets: [{
					label: label,
					pointRadius: 0,
					pointHoverRadius: 0,
					backgroundColor: bgColors,
					borderWidth: 0,
					data: data
				}]
			},
			options: {
				elements: {
					center: {
						text: textLabel,
						color: lineColor,
						sidePadding: 0
					}
				},
				cutoutPercentage: 90,
				legend: {

					display: false
				},

				tooltips: {
					enabled: false
				},

				scales: {
					yAxes: [{

						ticks: {
							display: false
						},
						gridLines: {
							drawBorder: false,
							zeroLineColor: "transparent",
							color: 'rgba(255,255,255,0.05)'
						}

					}],

					xAxes: [{
						barPercentage: 1.6,
						gridLines: {
							drawBorder: false,
							color: 'rgba(255,255,255,0.1)',
							zeroLineColor: "transparent"
						},
						ticks: {
							display: false,
						}
					}]
				},
			}
		});
	},

	shuffle: function (a) {
		var j, x, i;
		for (i = a.length - 1; i > 0; i--) {
			j = Math.floor(Math.random() * (i + 1));
			x = a[i];
			a[i] = a[j];
			a[j] = x;
		}
		return a;
	}
};