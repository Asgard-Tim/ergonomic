% 使用 importdata 函数读取文本文件
data = importdata('trajectory.txt');

% 获取 x 和 y 坐标
x = data(:, 1);
y = data(:, 3);

bg_img = imread('map.png');
imshow(bg_img, 'XData', [1.5 51.5], 'YData', [-18 32]);
axis on; % 显示坐标轴
hold on; % 保持图形，以便添加背景图像

scatter(x, y);

title('机器人轨迹记录');
