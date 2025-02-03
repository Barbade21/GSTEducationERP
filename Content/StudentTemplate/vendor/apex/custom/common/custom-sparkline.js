// Sparkline 1
var options1 = {
	series: [
		{
			name: "Visitors",
			data: [5, 10, 30, 15, 35, 25, 45],
		},
	],
	chart: {
		type: "line",
		width: 120,
		height: 100,
		sparkline: {
			enabled: true,
		},
	},
	stroke: {
		curve: "smooth",
		width: 3,
	},
	xaxis: {
		crosshairs: {
			width: 1,
		},
	},
	tooltip: {
		fixed: {
			enabled: false,
		},
	},
	colors: ["#a7874b"],
	xaxis: {
		categories: [
			"Sunday",
			"Monday",
			"Tuesday",
			"Wednesday",
			"Thursday",
			"Friday",
			"Saturday",
		],
	},
	tooltip: {
		y: {
			formatter: function (val) {
				return val + "k";
			},
		},
	},
};
var chart1 = new ApexCharts(document.querySelector("#visitors"), options1);
chart1.render();

// Sparkline 2
var options2 = {
	series: [
		{
			name: "Orders",
			data: [5, 10, 30, 15, 35, 25, 45],
		},
	],
	chart: {
		type: "line",
		width: 120,
		height: 100,
		sparkline: {
			enabled: true,
		},
	},
	stroke: {
		curve: "smooth",
		width: 3,
	},
	xaxis: {
		crosshairs: {
			width: 1,
		},
	},
	tooltip: {
		fixed: {
			enabled: false,
		},
	},
	colors: ["#d83628"],
	xaxis: {
		categories: [
			"Sunday",
			"Monday",
			"Tuesday",
			"Wednesday",
			"Thursday",
			"Friday",
			"Saturday",
		],
	},
	tooltip: {
		y: {
			formatter: function (val) {
				return val + "k";
			},
		},
	},
};
var chart2 = new ApexCharts(document.querySelector("#orders"), options2);
chart2.render();

// Sparkline 3
var options3 = {
	series: [
		{
			name: "Bookings",
			data: [5, 10, 30, 15, 35, 25, 45],
		},
	],
	chart: {
		type: "line",
		width: 120,
		height: 100,
		sparkline: {
			enabled: true,
		},
	},
	stroke: {
		curve: "smooth",
		width: 3,
	},
	xaxis: {
		crosshairs: {
			width: 1,
		},
	},
	tooltip: {
		fixed: {
			enabled: false,
		},
	},
	colors: ["#1fa278"],
	xaxis: {
		categories: [
			"Sunday",
			"Monday",
			"Tuesday",
			"Wednesday",
			"Thursday",
			"Friday",
			"Saturday",
		],
	},
	tooltip: {
		y: {
			formatter: function (val) {
				return val + "k";
			},
		},
	},
};
var chart3 = new ApexCharts(document.querySelector("#bookings"), options3);
chart3.render();

// Sparkline 4
var options4 = {
	series: [
		{
			name: "Subscribers",
			data: [5, 10, 30, 15, 35, 25, 45],
		},
	],
	chart: {
		type: "line",
		width: 120,
		height: 100,
		sparkline: {
			enabled: true,
		},
	},
	stroke: {
		curve: "smooth",
		width: 3,
	},
	xaxis: {
		crosshairs: {
			width: 1,
		},
	},
	tooltip: {
		fixed: {
			enabled: false,
		},
	},
	colors: ["#0078c7"],
	xaxis: {
		categories: [
			"Sunday",
			"Monday",
			"Tuesday",
			"Wednesday",
			"Thursday",
			"Friday",
			"Saturday",
		],
	},
	tooltip: {
		y: {
			formatter: function (val) {
				return val + "k";
			},
		},
	},
};
var chart4 = new ApexCharts(document.querySelector("#subscribers"), options4);
chart4.render();
