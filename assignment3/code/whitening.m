function [Y] = whitening( X )

[U,D] = eig(cov(X));

Y = sqrt(inv(D))* U' * X';

Y = Y';
end

