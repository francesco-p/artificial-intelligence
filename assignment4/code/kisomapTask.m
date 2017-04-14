%% K-Isomap 

%% Preprocessing
% We import the dataset, append the classification column and normalize
% data between 0 and 1

% Maximizes the difference between colors to best distinguish classes
colors = distinguishable_colors(10);

dataset = importdataset('Triesch_Dataset\*.pgm');

% Creates classification column
classification = repmat(1:10,1,24)';

% Images are ordered so we append the classification column
dataset = [dataset,classification]; 

% Normalization: I divide by 255 to simplify computations for distance matrix
n_dataset = dataset/255;

%% Distance matrix 
% We compute the Adjecency matrix with Euclidean Distance metric
distance_matrix = pdist2(n_dataset, n_dataset);

%% Isomap throug Stanford library

% Set first 10 dimensions to be used
options.dims = 1:10;
% No library's plot
options.overlay = 0;
options.display = 0;
    
figure;
i=1;
% K-Isomap as k varies
for k = [5:10,15,25,30,50]
    
    Y = Isomap(distance_matrix,'k',k,options);
    
    % It helps to restrict the plots, removes padding inside figure
    subplot_tight(5,2,i);
    % Data is inside Y.coords{2} where 2 are the dimensions of data
    scatter(Y.coords{2}(1,:),Y.coords{2}(2,:),10, classification, 'filled');
    title(strcat('Isomap K = ', num2str(k)));
    % Maps points to new colors
    colormap(colors);
    % Removes axis labels
    set(gca, 'XTick', []);
    set(gca, 'YTick', []);
    
    i=i+1;
end

%% CLassical MDS
% We apply classical multidimensional scaling for the sake of comparison

% Sqare each entry of the distance matrix, since MDS require so
distance_matrix= distance_matrix.^2;

% Apply MDS
YMDS = cmdscale(distance_matrix);

% Plot of data
figure;
scatter(YMDS(:,1),YMDS(:,2),10, classification, 'filled');
title('MDS');
colormap(colors);
colorbar;

%% 3D 5-Isomap
Y = Isomap(distance_matrix,'k',5,options);
figure;
scatter3(Y.coords{3}(1,:),Y.coords{3}(2,:),Y.coords{3}(3,:),10, classification, 'filled');
title('Isomap 3D embedding K = 5');
colormap(colors);
colorbar;
%% Non descriptive dimensions 5-Isomap
% We try to plot in two dimensions non descriptive dimensions 
figure;
scatter(Y.coords{9}(9,:),Y.coords{10}(10,:),10, classification, 'filled');
title('Isomap 2D embedding with not so descriptive dimensions');
colormap(colors);
colorbar;
