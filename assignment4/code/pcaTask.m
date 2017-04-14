%% Preprocessing
colors = distinguishable_colors(10);

dataset = importdataset('Triesch_Dataset\*.pgm');

dataset = dataset/255;

classification = repmat(1:10,1,24)';

%% PCA, first two components
coeff = pca(dataset);
f_two = coeff(:,1:2);

% Projection
reduction = dataset * f_two;
reduction = reduction';

% Centering
 reduction = myCenter(reduction); 

%% Plot
figure;
scatter(reduction(1,:), reduction(2,:),30,classification,'filled');
title('PCA');
%xlabel('First Principal Component');
%ylabel('Second Principal Component');
set(gca, 'XTick', []);
set(gca, 'YTick', []);
colormap(colors);
colorbar;

%% PCA, plot first three components

f_three = coeff(:,1:3);

% Projection
reduction = dataset * f_three;
reduction = reduction';

% Plot
figure;
scatter3(reduction(1,:), reduction(2,:), reduction(3,:), 30, classification, 'filled');
title('First 3 Principal Components');
colormap(colors);
colorbar;
