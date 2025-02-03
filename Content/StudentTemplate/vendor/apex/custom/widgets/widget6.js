// Chart 1
var options1 = {
	series: [
		{
			data: [
				15, 25, 35, 45, 55, 65, 75, 85, 95, 105, 115, 125, 135, 145, 95, 75, 55,
			],
		},
	],
	chart: {
		type: "bar",
		width: 120,
		height: 50,
		sparkline: {
			enabled: true,
		},
	},
	plotOptions: {
		bar: {
			columnWidth: "80%",
		},
	},
	xaxis: {
		crosshairs: {
			width: 1,
		},
	},
	colors: ["#3b5999", "#9196a2", "#66a4ff"],
	tooltip: {
		fixed: {
			enabled: false,
		},
		x: {
			show: false,
		},
		y: {
			title: {
				formatter: function (seriesName) {
					return "";
				},
			},
		},
		marker: {
			show: false,
		},
	},
};
var chart1 = new ApexCharts(document.querySelector("#bar1"), options1);
chart1.render();

// Chart 2
var options2 = {
	series: [
		{
			data: [
				15, 25, 35, 45, 55, 65, 75, 85, 95, 105, 115, 125, 135, 145, 95, 75, 55,
			],
		},
	],
	chart: {
		type: "bar",
		width: 120,
		height: 50,
		sparkline: {
			enabled: true,
		},
	},
	plotOptions: {
		bar: {
			columnWidth: "80%",
		},
	},
	xaxis: {
		crosshairs: {
			width: 1,
		},
	},
	colors: ["#0078c7", "#3b5999", "#66a4ff"],
	tooltip: {
		fixed: {
			enabled: false,
		},
		x: {
			show: false,
		},
		y: {
			title: {
				formatter: function (seriesName) {
					return "";
				},
			},
		},
		marker: {
			show: false,
		},
	},
};
var chart2 = new ApexCharts(document.querySelector("#bar2"), options2);
chart2.render();

// Chart 3
var options3 = {
	series: [
		{
			data: [
				15, 25, 35, 45, 55, 65, 75, 85, 95, 105, 115, 125, 135, 145, 95, 75, 55,
			],
		},
	],
	chart: {
		type: "bar",
		width: 120,
		height: 50,
		sparkline: {
			enabled: true,
		},
	},
	plotOptions: {
		bar: {
			columnWidth: "80%",
		},
	},
	xaxis: {
		crosshairs: {
			width: 1,
		},
	},
	colors: ["#fc7d1a", "#9196a2", "#66a4ff"],
	tooltip: {
		fixed: {
			enabled: false,
		},
		x: {
			show: false,
		},
		y: {
			title: {
				formatter: function (seriesName) {
					return "";
				},
			},
		},
		marker: {
			show: false,
		},
	},
};
var chart3 = new ApexCharts(document.querySelector("#bar3"), options3);
chart3.render();
