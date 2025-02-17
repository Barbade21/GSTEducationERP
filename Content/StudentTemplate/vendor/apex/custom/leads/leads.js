var options = {
	chart: {
		height: 320,
		type: "area",
		toolbar: {
			show: false,
		},
		dropShadow: {
			enabled: true,
			opacity: 0.1,
			blur: 5,
			left: -10,
			top: 10,
		},
	},
	dataLabels: {
		enabled: false,
	},
	stroke: {
		curve: "smooth",
		width: 3,
	},
	series: [
		{
			name: "Added",
			data: [300, 200, 400, 300, 500, 300, 400, 200, 300],
		},
		{
			name: "Converted",
			data: [150, 100, 200, 150, 250, 150, 200, 100, 150],
		},
	],
	grid: {
		borderColor: "#dfd6ff",
		strokeDashArray: 5,
		xaxis: {
			lines: {
				show: true,
			},
		},
		yaxis: {
			lines: {
				show: false,
			},
		},
		padding: {
			top: 0,
			right: 30,
			bottom: 0,
			left: 30,
		},
	},
	xaxis: {
		type: "day",
		categories: [
			"New",
			"Mid",
			"Long",
			"Short",
			"Low",
			"High",
			"Best",
			"Premium",
			"Amaze",
		],
	},
	colors: ["#c4c8cf", "#3b5999", "#EEEEEE", "#CCCCCC", "#3b5999", "#111111"],
	yaxis: {
		show: false,
	},
	markers: {
		size: 0,
		opacity: 0.2,
		colors: ["#c4c8cf", "#3b5999", "#EEEEEE", "#CCCCCC", "#3b5999", "#111111"],
		strokeColor: "#fff",
		strokeWidth: 2,
		hover: {
			size: 7,
		},
	},
	tooltip: {
		x: {
			format: "dd/MM/yy",
		},
	},
};

var chart = new ApexCharts(document.querySelector("#leads"), options);

chart.render();
