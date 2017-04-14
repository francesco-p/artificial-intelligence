function [ training_set, test_set ] = randomsamples( num, samples)
%--------------------------------------------------------------------------
% Syntax:       [ training_set, test_set ] = randomsamples( num, samples);

%
% Inputs:       num : the number of examples per 10 class
%               
%               samples : examples nxm matrix, the last column is the
%               classification
%
% Outputs:      training_set : the training set is a matrix composed by
%               num examples per 10 class
%
% Description:  This function returns num examples for each of the 10
%               classes
%
% Idea :        The function pick randomly 10 indexes of the matrix, it
%               if they are never been took I increment the count variable (1x10) to the
%               corresponding classification index, I repeat the process untill the whole
%               count array is filled with num which means that the function picked num
%               examples per class
%--------------------------------------------------------------------------


count = zeros(1,10);

% Just because the whole procedure is faster(uniform distribution of randi) I order data 
% by the classification
sorted = sortrows(samples,size(samples,2));

training_set = [];
all_indexes = [];

while ~all(count == num)
    
    % 10 random indexes
    indexes = randi([1,size(sorted,1)],1,10);    
    
    % Check for duplicate indexes
    indexes = setdiff(indexes, all_indexes);   
    all_indexes = [all_indexes, indexes];
    
    % Extract the examples
    extracted = sorted(indexes,:);

    % Foreach example
    for i = 1:size(indexes,2)
        obs = extracted(i,:);
        
        % Look the classification
        class = extracted(i,end);
        
        % Increment the corresponding cell of count
        cur_val = count(class+1);
        if cur_val < num
            training_set = [training_set;obs];
            count(class+1) = cur_val+1;
        end
    end
end

training_set = sortrows(training_set,size(samples,2));

% I create the test set by setdiff with the training
test_set = setdiff(samples,training_set,'rows');
end

