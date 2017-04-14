%% Laplacian Eigenmaps

%% Preprocessing
% We import the dataset, append the classification column and normalize
% data between 0 and 1

% Maximizes the difference between colors to best distinguish classes
colors = distinguishable_colors(10);

dataset = importdataset('Triesch_Dataset\*.pgm');

% Creates classification column
classification = repmat(1:10,1,24)';

% Normalization: I divide by 255 to simplify computations for distance matrix
n_dataset = dataset/255;

%% k-LEM

figure;
i=0;
for k = [5:10,15,25,30,50]
    i=i+1;
    
    [E,V] = leigs(n_dataset,'nn', k, 3);
    
    % It helps to restrict the plots, removes padding inside figure
    subplot_tight(5,2,i);
    scatter(E(:,1),E(:,2),10,classification,'filled');
    title(strcat('LEM K = ', num2str(k)))
    % Maps points to new colors
    colormap(colors);
    % Removes axis labels
    set(gca, 'XTick', []);
    set(gca, 'YTick', []);
   
end

