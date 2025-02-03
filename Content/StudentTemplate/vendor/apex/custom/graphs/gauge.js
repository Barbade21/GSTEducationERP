var options = {
	series: [75],
	chart: {
		height: 220,
		type: "radialBar",
		offsetY: -10,
	},
	plotOptions: {
		radialBar: {
			startAngle: -135,
			endAngle: 135,
			dataLabels: {
				name: {
					fontSize: "16px",
					color: undefined,
					offsetY: 120,
				},
				value: {
					offsetY: 76,
					fontSize: "21px",
					color: undefined,
					formatter: function (val) {
						return val + "%";
					},
				},
			},
		},
	},
	colors: [
		"#2f477a",
		"#35508a",
		"#3b5999",
		"#4f6aa3",
		"#627aad",
		"#768bb8",
		"#899bc2",
		"#9daccc",
	],
	stroke: {
		dashArray: 4,
	},
	labels: ["Sales Ratio"],
};

var chart = new ApexCharts(document.querySelector("#gauge"), options);
chart.render();
