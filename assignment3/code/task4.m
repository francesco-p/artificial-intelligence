%% Task 4
% *Problem:* _Test the performance of the Nearest Neighbor classifier using
% all the dimensions available, with independet components_.
%
% Estimated time is 29 seconds.

%% Data Loading
% We import the data, then we split it in observations and classification for the latter K-nn step. 
tic;
data = importdata('semeion.data');
classification = data(:,end); 
data = data(:,1:end-1);

%% Independent Component Analysis
% We perform the ICA with the full set of features, |myICA| needs a |nxd| matrix where where |d| are the samples, 
% that's why we transpose the input and the output.
 
z = myICA(data',256);
z = z';

%% K-NN Classification
% We perform classification with six different |k| values. Each
% classification instance accuracy is the average of 10 instances trained
% according to the problem.

accuracy = [];
avg_accuracies = [];

for k = [1 2 3:2:9] 
    
     
    % 10 different training and test sets
    for j = 1:10
      
        % Randomizing training set
        [training_set, test_set] = randomsamples(10,[z,classification]);
        mdl = fitcknn(training_set(:,1:256), training_set(:,end),'NumNeighbors',k);
        
        % Get the prediction
        prediction = predict(mdl, test_set(:,1:256));
        accuracy =[accuracy ; sum(prediction == test_set(:,end))/size(test_set,1)];
        
    end
    
        % Average accuracy of the 10 run
        avg = mean(accuracy);
        avg_accuracies =[avg_accuracies ,  avg];
        accuracy = [];
        fprintf('k = %d, IC = 256, Avg = %f \n ',k, avg);
    
end


%% Plot
% We plot the averages as |k| changes.

figure;
hold on;
    
for i = 1: size(avg_accuracies,1)
    plot(avg_accuracies(i,:));
end
    
hold off;
    
grid on;
set(gca, 'xtick', [1 2 3 4 5 6] );
set(gca, 'xticklabel', [1 2 3 5 7 9] );
xlabel('K');
ylabel('Average accuracy on 10 knn instances');
set(gca, 'yminortick','on');
set(gca, 'yminorgrid','on');
title('KNN and 256 IC');
toc
