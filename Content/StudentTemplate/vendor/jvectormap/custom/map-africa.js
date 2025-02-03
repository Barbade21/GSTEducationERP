// Africa
$(function () {
	$("#mapAfrica").vectorMap({
		map: "africa_mill",
		backgroundColor: "transparent",
		scaleColors: ["#a7874b"],
		zoomOnScroll: false,
		zoomMin: 1,
		hoverColor: true,
		series: {
			regions: [
				{
					values: gdpData,
					scale: ["#a7874b"],
					normalizeFunction: "polynomial",
				},
			],
		},
	});
});
