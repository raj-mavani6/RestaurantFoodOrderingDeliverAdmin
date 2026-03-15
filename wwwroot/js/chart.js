/**
 * Chart.js Integration for Report Section
 * Requires Chart.js library to be loaded first
 */

// Common Chart Options
const commonOptions = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
        legend: {
            display: false // We use custom legends usually, or set true if needed
        },
        tooltip: {
            backgroundColor: 'rgba(0, 0, 0, 0.8)',
            padding: 12,
            titleFont: { size: 13 },
            bodyFont: { size: 13 },
            cornerRadius: 6,
            displayColors: true
        }
    },
    scales: {
        y: {
            beginAtZero: true,
            grid: {
                borderDash: [5, 5],
                color: 'rgba(0, 0, 0, 0.05)'
            }
        },
        x: {
            grid: {
                display: false
            }
        }
    },
    elements: {
        line: {
            tension: 0.4 // Smooth curves
        },
        point: {
            radius: 4,
            hoverRadius: 6
        }
    }
};

/**
 * Initialize a Bar Chart
 * @param {string} canvasId - ID of the canvas element
 * @param {Array} labels - X-axis labels
 * @param {Array} data - Data points
 * @param {string} label - Dataset label
 * @param {string} color - Bar color
 */
function initBarChart(canvasId, labels, data, label, color = '#3b82f6') {
    const ctx = document.getElementById(canvasId);
    if (!ctx) return;

    return new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: label,
                data: data,
                backgroundColor: color,
                borderRadius: 4,
                borderWidth: 0
            }]
        },
        options: {
            ...commonOptions,
            plugins: {
                ...commonOptions.plugins,
                legend: { display: false }
            }
        }
    });
}

/**
 * Initialize a Line Chart
 * @param {string} canvasId - ID of the canvas element
 * @param {Array} labels - X-axis labels
 * @param {Array} data - Data points
 * @param {string} label - Dataset label
 * @param {string} color - Line color
 */
function initLineChart(canvasId, labels, data, label, color = '#3b82f6') {
    const ctx = document.getElementById(canvasId);
    if (!ctx) return;

    return new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: label,
                data: data,
                borderColor: color,
                backgroundColor: hexToRgba(color, 0.1),
                fill: true,
                borderWidth: 2
            }]
        },
        options: commonOptions
    });
}

/**
 * Initialize a Doughnut Chart
 * @param {string} canvasId - ID of the canvas element
 * @param {Array} labels - Labels
 * @param {Array} data - Data points
 * @param {Array} colors - Array of colors
 */
function initDoughnutChart(canvasId, labels, data, colors) {
    const ctx = document.getElementById(canvasId);
    if (!ctx) return;

    return new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: labels,
            datasets: [{
                data: data,
                backgroundColor: colors,
                borderWidth: 0
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'bottom',
                    labels: {
                        usePointStyle: true,
                        padding: 20
                    }
                }
            },
            cutout: '75%'
        }
    });
}

// Utility: Convert Hex to RGBA
function hexToRgba(hex, alpha) {
    const r = parseInt(hex.slice(1, 3), 16);
    const g = parseInt(hex.slice(3, 5), 16);
    const b = parseInt(hex.slice(5, 7), 16);
    return `rgba(${r}, ${g}, ${b}, ${alpha})`;
}
